using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using LegalAI.Shared.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAI.Service.Services.AIProvider;

public class QwenProvider : IAIProvider
{
    private readonly ILogger<QwenProvider> _logger;
    private readonly string? _apiKey;
    private readonly string _endpoint;
    private readonly bool _isEnabled;

    public string Name => "Qwen";
    public bool IsEnabled => _isEnabled;

    public QwenProvider(ILogger<QwenProvider> logger, IOptionsMonitor<AIConfig> config)
    {
        _logger = logger;
        var qwenConfig = config.CurrentValue.Qwen; // 直接访问 Qwen 配置
        _apiKey = qwenConfig?.ApiKey;
        _isEnabled = qwenConfig?.Enabled == true && !string.IsNullOrEmpty(_apiKey);
        _endpoint = qwenConfig?.Endpoint ?? "https://dashscope.aliyuncs.com/";
    }

    public async Task<AIResponse> GetResponseAsync(AIRequest request, CancellationToken ct = default)
    {
        if (!IsEnabled)
            return new AIResponse { IsError = true, ErrorMessage = "Qwen 未启用或 API Key 未配置" };

        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_endpoint);
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        try
        {
            var payload = new
            {
                model = "qwen-max",
                input = new
                {
                    prompt = $"请作为专业律师回答：{request.Question}"
                },
                parameters = new
                {
                    max_tokens = 1500,
                    temperature = 0.3
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("https://dashscope.aliyuncs.com/api/v1/services/aigc/text-generation/generation", content, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                _logger.LogError("Qwen API Error: {StatusCode} - {Error}", response.StatusCode, errorMsg);
                return new AIResponse { IsError = true, ErrorMessage = $"Qwen 服务错误" };
            }

            // 🔍 解析阿里云响应格式（简化版）
            var result = await response.Content.ReadFromJsonAsync<QwenResponse>();
            var answer = result?.Output?.Text ?? "AI 返回空结果";

            return new AIResponse
            {
                Answer = answer,
                Sources = ExtractSources(answer),
                ProviderName = Name
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Qwen 调用异常");
            return new AIResponse { IsError = true, ErrorMessage = ex.Message };
        }
    }

    private static List<string> ExtractSources(string answer)
    {
        // 通义千问可能返回不同格式，按需调整
        var sources = new List<string>();
        var pattern = @"《([^》]+)》第(\d+)条";
        var matches = System.Text.RegularExpressions.Regex.Matches(answer, pattern);
        foreach (System.Text.RegularExpressions.Match m in matches)
        {
            sources.Add($"《{m.Groups[1]}》第{m.Groups[2]}条");
        }
        return sources;
    }

    private class QwenResponse
    {
        public Output? Output { get; set; }
    }

    private class Output
    {
        public string? Text { get; set; }
    }
}