using LegalAI.Service.Services.AIProvider;

namespace LegalAI.Service.Factory;

public class AIProviderFactory
{
    private readonly Dictionary<string, IAIProvider> _providers;
    private readonly Random _random = new();

    public AIProviderFactory(IEnumerable<IAIProvider> providers)
    {
        _providers = providers.Where(p => p.IsEnabled).ToDictionary(p => p.Name, p => p);
    }

    public IAIProvider? GetProvider(string? providerName = null)
    {
        if (string.IsNullOrEmpty(providerName))
        {
            // 默认返回第一个可用的（或随机，按需调整）
            return _providers.Values.FirstOrDefault();
        }

        return _providers.GetValueOrDefault(providerName);
    }

    public List<string> GetAvailableProviders() => _providers.Keys.ToList();

    public IAIProvider GetRandomProvider()
    {
        var enabledProviders = _providers.Values.ToList();
        return enabledProviders.Count > 0 ? enabledProviders[_random.Next(enabledProviders.Count)] : null;
    }
}