using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;
using MarutiTrainingPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarutiTrainingPortal.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ContactService _contactService;
        private readonly IHtmlSanitizerService _htmlSanitizer;
        private readonly IRateLimitService _rateLimiter;
        private readonly ReCaptchaService _reCaptchaService;

        public ContactController(
            ApplicationDbContext context, 
            ContactService contactService,
            IHtmlSanitizerService htmlSanitizer,
            IRateLimitService rateLimiter,
            ReCaptchaService reCaptchaService)
        {
            _context = context;
            _contactService = contactService;
            _htmlSanitizer = htmlSanitizer;
            _rateLimiter = rateLimiter;
            _reCaptchaService = reCaptchaService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var profile = await _context.Profiles.FindAsync(1);
            return View(profile);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Booking()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None, Duration = 0)]
        public async Task<IActionResult> Submit([FromForm] ContactFormModel model)
        {
            // Prevent caching of contact form submissions
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate, private";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            // Verify reCAPTCHA FIRST (before rate limiting to prevent abuse)
            var recaptchaResponse = Request.Form["g-recaptcha-response"].ToString();
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            
            var (isValid, recaptchaError) = await _reCaptchaService.VerifyAsync(recaptchaResponse, remoteIp);
            
            if (!isValid)
            {
                // AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.ContentType?.Contains("application/json") == true)
                {
                    return Json(new { success = false, message = recaptchaError });
                }
                
                TempData["ErrorMessage"] = recaptchaError;
                return RedirectToAction(nameof(Index));
            }

            // Rate limiting: 10 requests per 10 minutes per IP (increased for testing)
            var identifier = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            if (!_rateLimiter.IsAllowed(identifier, maxRequests: 10, window: TimeSpan.FromMinutes(10)))
            {
                var errorMsg = "Too many requests. Please try again in a few minutes.";
                
                // AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.ContentType?.Contains("application/json") == true)
                {
                    return Json(new { success = false, message = errorMsg });
                }
                
                TempData["ErrorMessage"] = errorMsg;
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var errorMessage = string.Join(", ", errors);

                // AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.ContentType?.Contains("application/json") == true)
                {
                    return Json(new { success = false, message = errorMessage });
                }

                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction(nameof(Index));
            }

            // Sanitize user input to prevent XSS
            model.Name = _htmlSanitizer.Sanitize(model.Name);
            model.Subject = _htmlSanitizer.Sanitize(model.Subject);
            model.Message = _htmlSanitizer.Sanitize(model.Message);
            if (!string.IsNullOrEmpty(model.Company))
                model.Company = _htmlSanitizer.Sanitize(model.Company);

            // Capture IP and User Agent
            var sourceIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();

            try
            {
                // Submit via ContactService
                var (success, message) = await _contactService.SubmitContactMessageAsync(model, sourceIp, userAgent);

                // AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.ContentType?.Contains("application/json") == true)
                {
                    return Json(new { success, message });
                }

                // Regular form submission
                if (success)
                {
                    TempData["SuccessMessage"] = message;
                }
                else
                {
                    TempData["ErrorMessage"] = message;
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var errorMsg = "An unexpected error occurred. Please try again or contact us directly.";
                
                // Log the error
                Console.WriteLine($"Contact form error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // AJAX request
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.ContentType?.Contains("application/json") == true)
                {
                    return Json(new { success = false, message = errorMsg });
                }

                TempData["ErrorMessage"] = errorMsg;
                return RedirectToAction(nameof(Index));
            }
        }

        // Keep legacy endpoint for backwards compatibility
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(ContactMessage message)
        {
            // Redirect to new Submit action
            var model = new ContactFormModel
            {
                Name = message.Name,
                Email = message.Email,
                Phone = message.PhoneNumber,
                Company = message.Company,
                Subject = message.Subject,
                Message = message.Message
            };

            return await Submit(model);
        }
    }
}