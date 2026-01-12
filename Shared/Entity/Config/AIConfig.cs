namespace LegalAI.Shared.Entity.Config;

public class AIConfig
{
    public DeepSeekConfig? DeepSeek { get; set; }
    public QwenConfig? Qwen { get; set; }
    // 可继续添加其他模型配置
}

public class DeepSeekConfig
{
    public string? ApiKey { get; set; }
    public bool Enabled { get; set; } = false;
    public string? Endpoint { get; set; } = "https://api.deepseek.com/";
}

public class QwenConfig
{
    public string? ApiKey { get; set; }
    public bool Enabled { get; set; } = false;
    public string? Endpoint { get; set; } = "https://dashscope.aliyuncs.com/";
}