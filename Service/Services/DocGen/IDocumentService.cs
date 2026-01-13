using LegalAI.Shared.Models.Input;
using LegalAI.Shared.Models.Result;

namespace LegalAI.Service.Services.DocGen;

public interface IDocumentService
{
    /// <summary>
    /// ???????
    /// </summary>
    Task<AIResponse> GenerateComplaintAsync(AIRequest request, CancellationToken ct = default);
}
