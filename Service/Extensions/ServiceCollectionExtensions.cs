using LegalAI.Service.Factory;
using LegalAI.Service.Services.AIProvider;
using LegalAI.Service.Services.AskAI;
using LegalAI.Service.Services.DocGen;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAI.Service.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration config)
    {
        // 注册 Providers
        services.AddScoped<IAIProvider, DeepSeekProvider>();
        services.AddScoped<IAIProvider, QwenProvider>();

        // 注册工厂和协调器
        services.AddScoped<AIProviderFactory>();
        services.AddScoped<AIOrchestrator>();

        // 注册 services
        services.AddScoped<IAskAIService, AskAIService>();
        services.AddScoped<IDocumentService, DocumentService>();

        return services;
    }
}