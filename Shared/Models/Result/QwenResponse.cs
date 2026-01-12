namespace LegalAI.Shared.Models.Result
{
    public class QwenResponse
    {
        public Output? Output { get; set; }
    }

    public class Output
    {
        public string? Text { get; set; }
    }
}
