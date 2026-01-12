using LegalAI.Server.Services;
using LegalAI.Service.Factory;
using LegalAI.Service.Interfaces;
using LegalAI.Service.IServices;
using LegalAI.Service.Providers;
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
        services.AddScoped<IAIService, AIService>();

        return services;
    }
}