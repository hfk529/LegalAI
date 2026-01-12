using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegalAI.Shared.Models.Result
{
    public class DeepSeekResponse
    {
        public List<Choice>? Choices { get; set; }
    }

    public class Choice
    {
        public Message? Message { get; set; }
    }

    public class Message
    {
        public string? Content { get; set; }
    }
}
