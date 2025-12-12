using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MarutiTrainingPortal.Services
{
    /// <summary>
    /// Chatbot service with two modes:
    /// 1. LocalMock - Free, uses seeded Q&A (default)
    /// 2. OpenAI - OPTIONAL: Requires Azure OpenAI or OpenAI API key (may incur costs)
    /// 
    /// Configure via appsettings:
    /// "Chatbot": {
    ///   "Mode": "LocalMock" or "OpenAI",
    ///   "ApiKey": "your-key-here",
    ///   "Endpoint": "https://your-resource.openai.azure.com/" (for Azure) or null (for OpenAI)
    /// }
    /// </summary>
    public class ChatbotService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ChatbotService> _logger;
        private readonly string _mode;

        // Free fallback Q&A database
        private static readonly Dictionary<string, string> MockResponses = new()
        {
            { "hello", "Hello! I'm the Maruti Training Portal assistant. How can I help you today?" },
            { "hi", "Hi there! Welcome to Maruti Training Portal. What would you like to know?" },
            { "courses", "We offer courses in Azure Cloud, AI & Machine Learning, DevOps, and Cloud Architecture. Visit our Courses page to see all available options!" },
            { "training", "We provide corporate training in Azure, AI, and DevOps. Check out our Past Trainings page to see what we've delivered to companies." },
            { "events", "We regularly host webinars, workshops, and conferences. Visit our Events page to see upcoming sessions and register!" },
            { "certificate", "You can verify your training certificate on our Verify page. Just enter your certificate ID!" },
            { "contact", "You can reach us through our Contact page, or directly via email and WhatsApp. We're here to help!" },
            { "pricing", "Course pricing varies by topic and duration. Please visit the Courses page for specific pricing, or contact us for custom corporate training quotes." },
            { "instructor", "Our trainings are led by Maruti Makwana, an Azure Certified Trainer and AI Specialist with experience training 3000+ Students." },
            { "help", "I can help you with:\n- Finding courses and trainings\n- Upcoming events\n- Certificate verification\n- Contact information\n\nWhat would you like to know?" }
        };

        public ChatbotService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<ChatbotService> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _mode = configuration["Chatbot:Mode"] ?? "LocalMock";
        }

        public async Task<ChatbotResponse> GetResponseAsync(string userMessage)
        {
            try
            {
                if (_mode == "OpenAI")
                {
                    // OPTIONAL: May incur cost - requires API key
                    return await GetOpenAIResponseAsync(userMessage);
                }
                else
                {
                    // Free fallback
                    return GetMockResponse(userMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chatbot response");
                return new ChatbotResponse
                {
                    Message = "I'm having trouble responding right now. Please try the Contact page for assistance.",
                    Success = false
                };
            }
        }

        // Alias method for compatibility with ChatbotController
        public async Task<string> GetAnswerAsync(string question, List<string> conversationHistory)
        {
            var response = await GetResponseAsync(question);
            return response.Message;
        }

        private ChatbotResponse GetMockResponse(string userMessage)
        {
            var lowerMessage = userMessage.ToLower();

            // Find best matching response
            foreach (var kvp in MockResponses)
            {
                if (lowerMessage.Contains(kvp.Key))
                {
                    return new ChatbotResponse
                    {
                        Message = kvp.Value,
                        Success = true,
                        Source = "LocalMock"
                    };
                }
            }

            // Default response
            return new ChatbotResponse
            {
                Message = "I'm not sure about that. Could you try asking about courses, trainings, events, certificates, or contact information?",
                Success = true,
                Source = "LocalMock"
            };
        }

        // OPTIONAL: May incur cost
        private async Task<ChatbotResponse> GetOpenAIResponseAsync(string userMessage)
        {
            var apiKey = _configuration["Chatbot:ApiKey"];
            var endpoint = _configuration["Chatbot:Endpoint"];

            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("OpenAI mode configured but API key missing. Falling back to LocalMock.");
                return GetMockResponse(userMessage);
            }

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var isAzure = !string.IsNullOrEmpty(endpoint);
            var url = isAzure
                ? $"{endpoint}/openai/deployments/gpt-35-turbo/chat/completions?api-version=2023-05-15"
                : "https://api.openai.com/v1/chat/completions";

            if (isAzure)
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
                httpClient.DefaultRequestHeaders.Add("api-key", apiKey);
            }

            var requestBody = new
            {
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "You are a helpful assistant for Maruti Training Portal, specializing in Azure Cloud, AI, and DevOps training. Keep responses concise and helpful."
                    },
                    new
                    {
                        role = "user",
                        content = userMessage
                    }
                },
                max_tokens = 150,
                temperature = 0.7
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OpenAIResponse>(responseJson);

            return new ChatbotResponse
            {
                Message = result?.Choices?.FirstOrDefault()?.Message?.Content ?? "No response from AI",
                Success = true,
                Source = isAzure ? "AzureOpenAI" : "OpenAI"
            };
        }
    }

    public class ChatbotResponse
    {
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Source { get; set; } = string.Empty;
    }

    // OpenAI API response models
    public class OpenAIResponse
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
