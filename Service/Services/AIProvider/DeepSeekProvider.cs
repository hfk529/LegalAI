using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using LegalAI.Shared.Entity.Config;
using LegalAI.Shared.Models.Input;
using LegalAI.Shared.Models.Result;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAI.Service.Services.AIProvider;

public class DeepSeekProvider : IAIProvider
{
    private readonly ILogger<DeepSeekProvider> _logger;
    private readonly string? _apiKey;
    private readonly string _endpoint;
    private readonly bool _isEnabled;

    public string Name => "DeepSeek";
    public bool IsEnabled => _isEnabled;

    public DeepSeekProvider(ILogger<DeepSeekProvider> logger, IOptionsMonitor<AIConfig> config)
    {
        _logger = logger;
        var deepSeekConfig = config.CurrentValue.DeepSeek; // 直接访问 DeepSeek 配置
        _apiKey = deepSeekConfig?.ApiKey;
        _isEnabled = deepSeekConfig?.Enabled == true && !string.IsNullOrEmpty(_apiKey);
        _endpoint = deepSeekConfig?.Endpoint ?? "https://api.deepseek.com/";
    }

    public async Task<AIResponse> GetResponseAsync(AIRequest request, CancellationToken ct = default)
    {
        if (!IsEnabled)
            return new AIResponse { IsError = true, ErrorMessage = "DeepSeek 未启用或 API Key 未配置" };

        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_endpoint);
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        try
        {
            var systemPrompt = GetLegalPrompt(request.ModelType);
            var messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = $"用户问题：{request.Question}" }
            };

            var payload = new
            {
                model = "deepseek-reasoner",
                messages,
                max_tokens = 1500,
                temperature = 0.3,
                stream = false
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var response = await httpClient.PostAsync("https://api.deepseek.com/chat/completions", content, ct);
            stopwatch.Stop();

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                _logger.LogError("DeepSeek API Error: {StatusCode} - {Error}", response.StatusCode, errorMsg);
                return new AIResponse
                {
                    IsError = true,
                    ErrorMessage = $"DeepSeek 服务错误：{response.StatusCode}"
                };
            }

            var result = await response.Content.ReadFromJsonAsync<DeepSeekResponse>();
            var answer = result?.Choices?.FirstOrDefault()?.Message?.Content ?? "AI 返回空结果";

            return new AIResponse
            {
                Answer = answer,
                Sources = ExtractSources(answer),
                ProviderName = Name,
                LatencyMs = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeepSeek 调用异常");
            return new AIResponse { IsError = true, ErrorMessage = ex.Message };
        }
    }

    private static string GetLegalPrompt(string modelType)
    {
        // 根据 modelType 区分：咨询（"legal"）与文书生成（以 "doc" 开头）
        if (string.IsNullOrEmpty(modelType) || modelType == "legal")
        {
            return @"
                你是一名中国执业律师，严格依据现行有效的《中华人民共和国民法典》《刑法》《劳动合同法》等法律法规回答问题。
                要求：
                1. 若问题超出法律范围，回答“该问题不属于法律咨询范畴”；
                2. 若信息不足，回答“无法确定，请补充细节”，禁止编造；
                3. 所有结论必须引用具体法律条文（格式：《XXX法》第XX条）；
                4. 禁止使用“我认为”“建议你”等主观表述；
                5. 结尾固定加注：⚠️ 本回答由AI生成，仅供参考，不构成律师意见。
                ";
        }

        if (modelType.StartsWith("doc"))
        {
            // 文书生成的专用 prompt：输出规范格式的起诉状/文书，避免将其视为咨询回答
            return @"
                你是资深律师助理，负责根据用户提供的案情要素生成结构清晰、格式规范的民事起诉状。输出应为法律文书正文，不包含法律咨询的免责声明或主观建议。
                要求：
                1. 使用正式、规范的法律文书语言；
                2. 输出结构必须包含并按顺序给出：
                   - 标题（例如：民事起诉状），
                   - 受理法院，
                   - 当事人信息（原告、被告，含联系方式或住所/营业地址占位），
                   - 案由，
                   - 事实与理由（条理清晰，可分段），
                   - 证据目录（列出证据名称及证明目的），
                   - 诉讼请求（数字化表述，如请求判令 XXX），
                   - 落款（原告或代理人签名，日期），
                   - 送达信息（被告送达地址）。
                3. 如用户提供信息不足，应在相应位置以“[需补充：xxx]”标注需要补充的要点；禁止捏造事实。
                4. 对于金额、证据编号等敏感字段使用方括号占位（例如：[诉讼请求金额]、[证据1]）。
                5. 如需引用法律条文以支持主张，可在“事实与理由”或“证据”中简要标注法律条文，但无需在文末添加咨询类免责声明。
                6. 输出时不要包含多余的说明、步骤或给出如何维权的建议，只输出最终可作为提交法院的起诉状文本。
                ";
        }

        return "请根据用户问题提供专业回答。";
    }

    private static List<string> ExtractSources(string answer)
    {
        var sources = new List<string>();
        var pattern = @"《([^》]+)》第(\d+)条";
        var matches = System.Text.RegularExpressions.Regex.Matches(answer, pattern);
        foreach (System.Text.RegularExpressions.Match m in matches)
        {
            sources.Add($"《{m.Groups[1]}》第{m.Groups[2]}条");
        }
        return sources.Distinct().ToList();
    }
    
}