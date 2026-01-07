using System.Text.Json.Serialization;

namespace MarutiTrainingPortal.Models
{
    public class GeminiRequest
    {
        public List<GeminiContent> Contents { get; set; } = new();
        public GeminiGenerationConfig? GenerationConfig { get; set; }
    }

    public class GeminiContent
    {
        [JsonPropertyName("parts")]
        public List<GeminiPart> Parts { get; set; } = new();
        
        [JsonPropertyName("role")]
        public string? Role { get; set; }
    }

    public class GeminiPart
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    public class GeminiGenerationConfig
    {
        public double Temperature { get; set; } = 0.7;
        public int MaxOutputTokens { get; set; } = 2048;
    }

    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<GeminiCandidate>? Candidates { get; set; }
        
        [JsonPropertyName("promptFeedback")]
        public GeminiPromptFeedback? PromptFeedback { get; set; }
    }

    public class GeminiCandidate
    {
        [JsonPropertyName("content")]
        public GeminiContent? Content { get; set; }
        
        [JsonPropertyName("finishReason")]
        public string? FinishReason { get; set; }
    }

    public class GeminiPromptFeedback
    {
        [JsonPropertyName("safetyRatings")]
        public List<GeminiSafetyRating>? SafetyRatings { get; set; }
    }

    public class GeminiSafetyRating
    {
        [JsonPropertyName("category")]
        public string? Category { get; set; }
        
        [JsonPropertyName("probability")]
        public string? Probability { get; set; }
    }
}
