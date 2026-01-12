using LegalAI.Service.Services.AskAI;
using LegalAI.Shared.Models.Input;
using LegalAI.Shared.Models.Result;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LegalAI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;

        public AIController(IAIService aiService)
        {
            _aiService = aiService;
        }

        /// <summary>
        /// 提交法律问题咨询
        /// </summary>
        /// <param name="request">咨询请求对象</param>
        /// <returns>AI 回答结果</returns>
        /// <response code="200">咨询成功</response>
        /// <response code="400">请求参数错误</response>
        /// <response code="500">服务器内部错误</response>  
        [HttpPost("consult")]
        [ProducesResponseType(typeof(AIResponse), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<ActionResult<AIResponse>> Consult([FromBody] AIRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest("问题不能为空");

            // 🛡️ 简单限流：同一用户 5 秒内只能问 1 次
            var userId = User.Identity?.Name ?? "anonymous";
            var cacheKey = $"ai_rate_limit:{userId}";

            if (HttpContext.RequestServices.GetService<IMemoryCache>()?.Get(cacheKey) != null)
                return TooManyRequests("请 5 秒后再提问");

            var response = await _aiService.GetLegalAnswerAsync(new AIRequest
            {
                Question = request.Question,
                UserId = userId
            });

            // 设置限流标记
            HttpContext.RequestServices.GetService<IMemoryCache>()
                ?.Set(cacheKey, true, TimeSpan.FromSeconds(5));

            return response;
        }

        private ActionResult TooManyRequests(string message)
            => StatusCode(429, new AIResponse { IsError = true, ErrorMessage = message });
    }
}
