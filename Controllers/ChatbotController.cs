using Microsoft.AspNetCore.Mvc;
using MarutiTrainingPortal.Services;
using System.ComponentModel.DataAnnotations;

namespace MarutiTrainingPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly ChatbotService _chatbotService;
        private readonly ILogger<ChatbotController> _logger;

        public ChatbotController(ChatbotService chatbotService, ILogger<ChatbotController> logger)
        {
            _chatbotService = chatbotService;
            _logger = logger;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatbotRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { error = "Invalid request" });
                }

                if (string.IsNullOrWhiteSpace(request.Question))
                {
                    return BadRequest(new { error = "Question is required" });
                }

                // Limit question length to prevent abuse
                if (request.Question.Length > 500)
                {
                    return BadRequest(new { error = "Question too long (max 500 characters)" });
                }

                var answer = await _chatbotService.GetAnswerAsync(
                    request.Question, 
                    request.ConversationHistory ?? new List<string>()
                );

                return Ok(new { answer });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chatbot request");
                return StatusCode(500, new { error = "An error occurred processing your request" });
            }
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", mode = "LocalMock" });
        }
    }

    public class ChatbotRequest
    {
        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Question { get; set; } = string.Empty;

        public List<string>? ConversationHistory { get; set; }
    }
}
