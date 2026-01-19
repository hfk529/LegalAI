using LegalAI.Service.Factory;
using LegalAI.Shared.Models.Input;
using LegalAI.Shared.Models.Result;
using Microsoft.Extensions.Logging;

namespace LegalAI.Service.Services.DocGen;

public class DocumentService : IDocumentService
{
    private readonly ILogger<DocumentService> _logger;
    private readonly AIOrchestrator _orchestrator;

    public DocumentService(ILogger<DocumentService> logger, AIOrchestrator orchestrator)
    {
        _logger = logger;
        _orchestrator = orchestrator;
    }

    public async Task<AIResponse> GenerateComplaintAsync(AIRequest request, CancellationToken ct = default)
    {
        try
        {
            // 将 ModelType 标记为文书生成，以便 Orchestrator/Provider 根据类型选择不同的 prompt 或模型
            request.ModelType = "doc:complaint";

            var response = await _orchestrator.GetResponseAsync(request, ct);

            return response;
        }
        catch (OperationCanceledException)
        {
            return new AIResponse { IsError = true, ErrorMessage = "文书生成超时，请重试" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "文书生成服务异常");
            return new AIResponse { IsError = true, ErrorMessage = "系统繁忙，文书生成失败" };
        }
    }
}
