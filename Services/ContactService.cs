using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;
using Microsoft.Extensions.Options;

namespace MarutiTrainingPortal.Services;

public class ContactService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ContactService> _logger;

    public ContactService(
        ApplicationDbContext context,
        IEmailSender emailSender,
        IConfiguration configuration,
        ILogger<ContactService> logger)
    {
        _context = context;
        _emailSender = emailSender;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<(bool Success, string Message)> SubmitContactMessageAsync(
        ContactFormModel model, 
        string? sourceIp, 
        string? userAgent)
    {
        try
        {
            _logger.LogInformation("Processing contact form submission from {Email}", model.Email);

            // Basic spam check - can be expanded
            if (!string.IsNullOrEmpty(model.Message) && ContainsSpamKeywords(model.Message) || 
                !string.IsNullOrEmpty(model.Subject) && ContainsSpamKeywords(model.Subject))
            {
                _logger.LogWarning("Spam detected from IP {IP}: {Email}", sourceIp, model.Email);
                return (false, "Your message could not be submitted. Please try again later.");
            }

            // Create ContactMessage entity
            var contactMessage = new ContactMessage
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.Phone,
                Company = model.Company,
                Subject = model.Subject,
                Message = model.Message,
                SourceIp = sourceIp,
                UserAgent = userAgent,
                Status = "New",
                IsRead = false
            };

            // Save to database
            _context.ContactMessages.Add(contactMessage);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Contact message saved to database: ID={Id}, Email={Email}", 
                contactMessage.Id, contactMessage.Email);

            // Send email notification to admin
            var adminEmail = _configuration["Contact:AdminEmail"];
            if (!string.IsNullOrEmpty(adminEmail))
            {
                var emailSubject = $"New Contact Form Submission: {model.Subject}";
                var emailBody = BuildEmailBody(model, sourceIp, userAgent, contactMessage.Id);
                
                var emailSent = await _emailSender.SendEmailAsync(adminEmail, "Admin", emailSubject, emailBody);
                
                if (emailSent)
                {
                    _logger.LogInformation("Email notification sent to admin for contact message ID={Id}", contactMessage.Id);
                }
                else
                {
                    _logger.LogWarning("Failed to send email notification for contact message ID={Id}", contactMessage.Id);
                }
            }

            return (true, "Thank you for contacting us! We'll get back to you shortly.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing contact form submission from {Email}. Error: {ErrorMessage}", 
                model?.Email ?? "unknown", ex.Message);
            return (false, "An error occurred while processing your message. Please try again later. If the problem persists, please contact us directly via email or phone.");
        }
    }

    private static bool ContainsSpamKeywords(string text)
    {
        var spamKeywords = new[] { "viagra", "casino", "lottery", "crypto", "bitcoin", "mlm" };
        return spamKeywords.Any(keyword => text.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    private static string BuildEmailBody(ContactFormModel model, string? sourceIp, string? userAgent, int messageId)
    {
        return $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background: #007bff; color: white; padding: 15px; border-radius: 5px 5px 0 0; }}
                    .content {{ background: #f9f9f9; padding: 20px; border: 1px solid #ddd; border-radius: 0 0 5px 5px; }}
                    .field {{ margin-bottom: 15px; }}
                    .label {{ font-weight: bold; color: #555; }}
                    .value {{ color: #333; }}
                    .metadata {{ background: #fff; padding: 10px; margin-top: 15px; border-left: 3px solid #007bff; }}
                    .footer {{ margin-top: 20px; padding-top: 15px; border-top: 1px solid #ddd; font-size: 12px; color: #666; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h2>New Contact Form Submission</h2>
                    </div>
                    <div class='content'>
                        <div class='field'>
                            <span class='label'>Name:</span>
                            <span class='value'>{System.Web.HttpUtility.HtmlEncode(model.Name)}</span>
                        </div>
                        <div class='field'>
                            <span class='label'>Email:</span>
                            <span class='value'><a href='mailto:{model.Email}'>{System.Web.HttpUtility.HtmlEncode(model.Email)}</a></span>
                        </div>
                        {(!string.IsNullOrEmpty(model.Phone) ? $@"
                        <div class='field'>
                            <span class='label'>Phone:</span>
                            <span class='value'>{System.Web.HttpUtility.HtmlEncode(model.Phone)}</span>
                        </div>" : "")}
                        {(!string.IsNullOrEmpty(model.Company) ? $@"
                        <div class='field'>
                            <span class='label'>Company:</span>
                            <span class='value'>{System.Web.HttpUtility.HtmlEncode(model.Company)}</span>
                        </div>" : "")}
                        <div class='field'>
                            <span class='label'>Subject:</span>
                            <span class='value'>{System.Web.HttpUtility.HtmlEncode(model.Subject)}</span>
                        </div>
                        <div class='field'>
                            <span class='label'>Message:</span>
                            <div class='value' style='white-space: pre-wrap;'>{System.Web.HttpUtility.HtmlEncode(model.Message)}</div>
                        </div>
                        
                        <div class='metadata'>
                            <div class='field'>
                                <span class='label'>Message ID:</span>
                                <span class='value'>{messageId}</span>
                            </div>
                            {(!string.IsNullOrEmpty(sourceIp) ? $@"
                            <div class='field'>
                                <span class='label'>IP Address:</span>
                                <span class='value'>{System.Web.HttpUtility.HtmlEncode(sourceIp)}</span>
                            </div>" : "")}
                            {(!string.IsNullOrEmpty(userAgent) ? $@"
                            <div class='field'>
                                <span class='label'>User Agent:</span>
                                <span class='value' style='font-size: 11px;'>{System.Web.HttpUtility.HtmlEncode(userAgent)}</span>
                            </div>" : "")}
                            <div class='field'>
                                <span class='label'>Submitted:</span>
                                <span class='value'>{DateTime.Now:yyyy-MM-dd HH:mm:ss}</span>
                            </div>
                        </div>
                    </div>
                    <div class='footer'>
                        <p>This is an automated notification from your website contact form.</p>
                    </div>
                </div>
            </body>
            </html>
        ";
    }
}
