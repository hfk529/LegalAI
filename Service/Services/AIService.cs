using LegalAI.Service.Factory;
using LegalAI.Service.IServices;
using LegalAI.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace LegalAI.Server.Services;

public class AIService : IAIService
{
    private readonly ILogger<AIService> _logger;
    private readonly AIOrchestrator _aiOrchestrator;

    public AIService(ILogger<AIService> logger, AIOrchestrator aIOrchestrator)
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
}