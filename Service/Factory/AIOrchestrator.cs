using LegalAI.Shared.Models;

namespace LegalAI.Service.Factory;

public class AIOrchestrator
{
    private readonly AIProviderFactory _factory;

    public AIOrchestrator(AIProviderFactory factory)
    {
        _factory = factory;
    }

    public async Task<AIResponse> GetResponseAsync(AIRequest request, CancellationToken ct = default)
    {
        var provider = _factory.GetProvider(request.ModelType == "legal" ? "DeepSeek" : null); // 法律场景优先用 DeepSeek

        if (provider == null)
        {
            return new AIResponse { IsError = true, ErrorMessage = "无可用 AI 服务" };
        }

        return await provider.GetResponseAsync(request, ct);
    }
}