using LegalAI.Service.Factory;
using LegalAI.Shared.Models.Input;
using LegalAI.Shared.Models.Result;
using Microsoft.Extensions.Logging;

namespace LegalAI.Service.Services.AskAI;

public class AskAIService : IAskAIService
{
    private readonly ILogger<AskAIService> _logger;
    private readonly AIOrchestrator _aiOrchestrator;

    public AskAIService(ILogger<AskAIService> logger, AIOrchestrator aIOrchestrator)
    {
        _logger = logger;
        _aiOrchestrator = aIOrchestrator;
    }

    public async Task<AIResponse> GetLegalAnswerAsync(AIRequest request, CancellationToken ct = default)
    {
        try
        {
            AIResponse aIResponse = await _aiOrchestrator.GetResponseAsync(request, ct);
            return aIResponse;
        }
        catch (OperationCanceledException)
        {
            return new AIResponse { IsError = true, ErrorMessage = "请求超时，请重试" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI 服务异常");
            return new AIResponse { IsError = true, ErrorMessage = "系统繁忙，请稍后再试" };
        }
    }

    public async Task<AIResponse> GenerateComplaintAsync(AIRequest request, CancellationToken ct = default)
    {
        try
        {
            // 使用 Orchestrator 生成起诉状，ModelType 指定为 legal:complaint
            request.ModelType = "legal:complaint";
            AIResponse aIResponse = await _aiOrchestrator.GetResponseAsync(request, ct);
            return aIResponse;
        }
        catch (OperationCanceledException)
        {
            return new AIResponse { IsError = true, ErrorMessage = "生成超时，请重试" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "起诉状生成服务异常");
            return new AIResponse { IsError = true, ErrorMessage = "系统繁忙，文书生成失败" };
        }
    }
}