using LegalAI.Shared.Models.Input;
using LegalAI.Shared.Models.Result;

namespace LegalAI.Service.Services.DocGen;

public interface IDocumentService
{
    /// <summary>
    /// 生成民事起诉状（根据请求中的案情要素生成规范、可提交法院的起诉状文本）
    /// </summary>
    Task<AIResponse> GenerateComplaintAsync(AIRequest request, CancellationToken ct = default);
}
