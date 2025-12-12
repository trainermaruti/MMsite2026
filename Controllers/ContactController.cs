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
        private readonly IEmailSender _emailSender;
        private readonly IHtmlSanitizerService _htmlSanitizer;
        private readonly IRateLimitService _rateLimiter;

        public ContactController(
            ApplicationDbContext context, 
            IEmailSender emailSender, 
            IHtmlSanitizerService htmlSanitizer,
            IRateLimitService rateLimiter)
        {
            _context = context;
            _emailSender = emailSender;
            _htmlSanitizer = htmlSanitizer;
            _rateLimiter = rateLimiter;
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
        public IActionResult SendMessage()
        {
            // Redirect GET requests to Index page
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(ContactMessage message)
        {
            // Rate limiting: 10 requests per hour per IP
            var identifier = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            if (!_rateLimiter.IsAllowed(identifier, maxRequests: 10, window: TimeSpan.FromHours(1)))
            {
                TempData["ErrorMessage"] = "Too many requests. Please try again later.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                // Sanitize user input to prevent XSS
                message.Name = _htmlSanitizer.Sanitize(message.Name);
                message.Subject = _htmlSanitizer.Sanitize(message.Subject);
                message.Message = _htmlSanitizer.Sanitize(message.Message);
                
                // Set the creation date
                message.CreatedDate = DateTime.UtcNow;
                message.IsRead = false;
                message.IsDeleted = false;

                _context.ContactMessages.Add(message);
                await _context.SaveChangesAsync();

                // Send email notification to admin
                try
                {
                    await _emailSender.SendEmailAsync(
                        "admin@marutitraining.com",
                        $"New Contact Message: {message.Subject}",
                        $@"<h3>New Contact Form Submission</h3>
                           <p><strong>From:</strong> {message.Name} ({message.Email})</p>
                           <p><strong>Subject:</strong> {message.Subject}</p>
                           <p><strong>Message:</strong></p>
                           <p>{message.Message}</p>
                           <p><em>Submitted on: {message.CreatedDate:yyyy-MM-dd HH:mm}</em></p>"
                    );
                }
                catch (Exception ex)
                {
                    // Log error but don't fail the request
                    Console.WriteLine($"Email sending failed: {ex.Message}");
                }

                TempData["SuccessMessage"] = "Your message has been sent successfully. We'll get back to you soon!";
                return RedirectToAction(nameof(Index));
            }

            // If ModelState is invalid, redirect to Index with error message
            TempData["ErrorMessage"] = "Please check the form and try again. All required fields must be filled.";
            return RedirectToAction(nameof(Index));
        }
    }
}
