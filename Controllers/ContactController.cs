using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;
using MarutiTrainingPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarutiTrainingPortal.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IHtmlSanitizerService _htmlSanitizer;
        private readonly IRateLimitService _rateLimiter;
        private readonly ILogger<ContactController> _logger;

        public ContactController(
            ApplicationDbContext context, 
            IEmailSender emailSender, 
            IHtmlSanitizerService htmlSanitizer,
            IRateLimitService rateLimiter,
            ILogger<ContactController> logger)
        {
            _context = context;
            _emailSender = emailSender;
            _htmlSanitizer = htmlSanitizer;
            _rateLimiter = rateLimiter;
            _logger = logger;
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
            _logger.LogInformation("Contact form submission received from {IP}", HttpContext.Connection.RemoteIpAddress);
            
            // Rate limiting: 10 requests per hour per IP
            var identifier = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            if (!_rateLimiter.IsAllowed(identifier, maxRequests: 10, window: TimeSpan.FromHours(1)))
            {
                _logger.LogWarning("Rate limit exceeded for IP: {IP}", identifier);
                TempData["ErrorMessage"] = "Too many requests. Please try again later.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("ModelState valid: {IsValid}", ModelState.IsValid);
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    _logger.LogWarning("Validation error: {ErrorMessage}", error.ErrorMessage);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitize user input to prevent XSS
                    message.Name = _htmlSanitizer.Sanitize(message.Name);
                    message.Subject = _htmlSanitizer.Sanitize(message.Subject);
                    message.Message = _htmlSanitizer.Sanitize(message.Message);
                    
                    // Set the creation date and defaults
                    message.CreatedDate = DateTime.UtcNow;
                    message.IsRead = false;
                    message.IsDeleted = false;
                    if (string.IsNullOrEmpty(message.Status))
                    {
                        message.Status = "New";
                    }

                    _context.ContactMessages.Add(message);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Message saved successfully. ID: {MessageId}, From: {Name}", message.Id, message.Name);

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
                catch (Exception ex)
                {
                    // Log the error
                    _logger.LogError(ex, "Error saving contact message: {ErrorMessage}", ex.Message);
                    TempData["ErrorMessage"] = $"Error saving message: {ex.Message}";
                    return RedirectToAction(nameof(Index));
                }
            }

            // If ModelState is invalid, redirect to Index with error message
            TempData["ErrorMessage"] = "Please check the form and try again. All required fields must be filled.";
            return RedirectToAction(nameof(Index));
        }

        // DEBUG: Check message count
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DebugMessages()
        {
            var allMessages = await _context.ContactMessages.ToListAsync();
            var nonDeletedMessages = await _context.ContactMessages.Where(m => !m.IsDeleted).ToListAsync();
            
            return Json(new
            {
                total = allMessages.Count,
                nonDeleted = nonDeletedMessages.Count,
                messages = nonDeletedMessages.Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.Email,
                    m.Subject,
                    m.CreatedDate,
                    m.IsDeleted,
                    m.IsRead
                }).ToList()
            });
        }

        // DEBUG: Create a test message
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DebugCreateMessage()
        {
            try
            {
                var testMessage = new ContactMessage
                {
                    Name = "Test User",
                    Email = "test@example.com",
                    Subject = "Test Message",
                    Message = "This is a test message",
                    PhoneNumber = "1234567890",
                    CreatedDate = DateTime.UtcNow,
                    IsRead = false,
                    IsDeleted = false,
                    Status = "New"
                };

                _context.ContactMessages.Add(testMessage);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Test message created",
                    id = testMessage.Id,
                    isDeleted = testMessage.IsDeleted
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        // DEBUG: Bypass query filter
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DebugAllMessages()
        {
            var allMessages = await _context.ContactMessages.IgnoreQueryFilters().ToListAsync();
            
            return Json(new
            {
                totalCount = allMessages.Count,
                messages = allMessages.Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.Email,
                    m.Subject,
                    m.CreatedDate,
                    m.IsDeleted,
                    m.IsRead,
                    m.Status
                }).ToList()
            });
        }

        // DEBUG: Show what admin portal will see
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DebugAdminView()
        {
            // This is what the admin will see (with query filter applied)
            var visibleMessages = await _context.ContactMessages.ToListAsync();
            
            // This is everything in the database
            var allMessages = await _context.ContactMessages.IgnoreQueryFilters().ToListAsync();
            
            return Json(new
            {
                visibleInAdminPortal = visibleMessages.Count,
                totalInDatabase = allMessages.Count,
                deleted = allMessages.Count(m => m.IsDeleted),
                notDeleted = allMessages.Count(m => !m.IsDeleted),
                visibleMessages = visibleMessages.Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.Email,
                    m.Subject,
                    m.CreatedDate,
                    m.IsDeleted,
                    m.IsRead,
                    m.Status
                }).OrderByDescending(m => m.CreatedDate).ToList(),
                allMessages = allMessages.Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.Email,
                    m.Subject,
                    m.CreatedDate,
                    m.IsDeleted,
                    m.IsRead,
                    m.Status
                }).OrderByDescending(m => m.CreatedDate).ToList()
            });
        }
    }
}

