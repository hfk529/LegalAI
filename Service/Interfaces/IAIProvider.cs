using LegalAI.Shared.Models;

namespace LegalAI.Service.Interfaces
{
    public interface IAIProvider
    {
        string Name { get; } // 例如 "DeepSeek", "Qwen", "LocalOllama"
        Task<AIResponse> GetResponseAsync(AIRequest request, CancellationToken ct = default);
        bool IsEnabled { get; } // 是否启用该模型
    }
}
