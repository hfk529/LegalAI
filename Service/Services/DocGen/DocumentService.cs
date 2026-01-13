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
            // ? ModelType ?????????? Orchestrator/Provider ???????? prompt ???
            request.ModelType = "doc:complaint";

            var response = await _orchestrator.GetResponseAsync(request, ct);

            return response;
        }
        catch (OperationCanceledException)
        {
            return new AIResponse { IsError = true, ErrorMessage = "??????????" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "??????");
            return new AIResponse { IsError = true, ErrorMessage = "???????????" };
        }
    }
}
