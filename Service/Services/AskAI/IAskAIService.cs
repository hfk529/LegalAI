using LegalAI.Shared.Models.Input;
using LegalAI.Shared.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalAI.Service.Services.AskAI
{
    public interface IAskAIService
    {
        Task<AIResponse> GetLegalAnswerAsync(AIRequest request, CancellationToken ct = default);
    }
}
