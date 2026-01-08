using LegalAI.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalAI.Service.IServices
{
    public interface IAIService
    {
        Task<AIResponse> GetLegalAnswerAsync(AIRequest request, CancellationToken ct = default);
    }
}
