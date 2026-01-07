using Microsoft.AspNetCore.Mvc;
using MarutiTrainingPortal.Models;
using MarutiTrainingPortal.Services;

namespace MarutiTrainingPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIChatController : ControllerBase
    {
        private readonly IGeminiService _geminiService;
        private readonly ICourseService _courseService;
        private readonly ILeadService _leadService;
        private readonly ILogger<AIChatController> _logger;

        public AIChatController(IGeminiService geminiService, ICourseService courseService, ILeadService leadService, ILogger<AIChatController> logger)
        {
            _geminiService = geminiService;
            _courseService = courseService;
            _leadService = leadService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ChatResponse>> SendMessage([FromBody] ChatRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return BadRequest(new ChatResponse
                    {
                        Success = false,
                        ErrorMessage = "Message cannot be empty"
                    });
                }

                var response = await _geminiService.SendMessageAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                return StatusCode(500, new ChatResponse
                {
                    Success = false,
                    ErrorMessage = "An error occurred while processing your request"
                });
            }
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }

        [HttpGet("courses")]
        public IActionResult GetCourses()
        {
            try
            {
                var catalog = _courseService.GetCatalog();
                return Ok(catalog.Courses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching courses");
                return StatusCode(500, new { error = "Failed to fetch courses" });
            }
        }

        [HttpGet("courses/{code}")]
        public IActionResult GetCourse(string code)
        {
            try
            {
                var course = _courseService.GetCourse(code);
                if (course == null)
                {
                    return NotFound(new { error = $"Course {code} not found" });
                }
                return Ok(course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching course");
                return StatusCode(500, new { error = "Failed to fetch course" });
            }
        }

        [HttpGet("learning-paths")]
        public IActionResult GetLearningPaths()
        {
            try
            {
                var catalog = _courseService.GetCatalog();
                return Ok(catalog.LearningPaths);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching learning paths");
                return StatusCode(500, new { error = "Failed to fetch learning paths" });
            }
        }

        [HttpGet("certifications")]
        public IActionResult GetCertifications()
        {
            try
            {
                var catalog = _courseService.GetCatalog();
                return Ok(catalog.Certifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching certifications");
                return StatusCode(500, new { error = "Failed to fetch certifications" });
            }
        }

        [HttpGet("products")]
        public IActionResult GetProducts()
        {
            try
            {
                var catalog = _courseService.GetCatalog();
                return Ok(catalog.Products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products");
                return StatusCode(500, new { error = "Failed to fetch products" });
            }
        }

        [HttpPost("capture-lead")]
        public async Task<IActionResult> CaptureLead([FromBody] LeadCapture lead)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(lead.Email))
                {
                    return BadRequest(new { error = "Email is required" });
                }

                var success = await _leadService.CaptureLeadAsync(lead);
                
                if (success)
                {
                    return Ok(new { 
                        message = "Lead captured successfully",
                        email = lead.Email,
                        interest = lead.Interest
                    });
                }

                return StatusCode(500, new { error = "Failed to capture lead" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing lead");
                return StatusCode(500, new { error = "Failed to capture lead" });
            }
        }

        [HttpGet("leads")]
        public async Task<IActionResult> GetLeads()
        {
            try
            {
                var leads = await _leadService.GetLeadsAsync();
                return Ok(leads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching leads");
                return StatusCode(500, new { error = "Failed to fetch leads" });
            }
        }
    }
}
