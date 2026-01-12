using LegalAI.Service.Factory;
using LegalAI.Service.Services.AIProvider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LegalAI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly AIProviderFactory _factory;
        private readonly IEnumerable<IAIProvider> _providers;
        private readonly IConfiguration _configuration;

        public TestController(AIProviderFactory factory, IEnumerable<IAIProvider> providers, IConfiguration configuration)
        {
            _factory = factory;
            _providers = providers;
            _configuration = configuration;
        }

        [HttpGet("providers")]
        public IActionResult GetProviders()
        {
            var allProviders = _providers.ToList();
            var availableProviders = _factory.GetAvailableProviders();

            return Ok(new
            {
                TotalProviders = allProviders.Count,
                AvailableProviders = availableProviders,
                FactoryProviderCount = _factory.GetAvailableProviders().Count
            });
        }

        [HttpGet("config")]
        public IActionResult GetConfig()
        {
            var deepseekEnabled = _configuration.GetValue<bool>("AIConfig:DeepSeek:Enabled");
            var qwenEnabled = _configuration.GetValue<bool>("AIConfig:Qwen:Enabled");

            return Ok(new
            {
                OpenAIEnabled = deepseekEnabled,
                AzureAIEnabled = qwenEnabled,
                FullConfig = _configuration.AsEnumerable().ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            });
        }

    }
}
