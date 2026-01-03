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
                var emailBody = BuildAdminEmailBody(model, sourceIp, userAgent, contactMessage.Id);
                
                // Set Reply-To as the user's email so admin can reply directly
                var emailSent = await _emailSender.SendEmailAsync(adminEmail, "Admin", emailSubject, emailBody, model.Email, model.Name);
                
                if (emailSent)
                {
                    _logger.LogInformation("Email notification sent to admin for contact message ID={Id}", contactMessage.Id);
                }
                else
                {
                    _logger.LogWarning("Failed to send email notification for contact message ID={Id}", contactMessage.Id);
                }
            }

            // Send confirmation email to sender
            try
            {
                var confirmationSubject = "Thank You for Contacting Maruti Makwana Training";
                var confirmationBody = BuildSenderConfirmationEmail(model);
                
                var confirmationSent = await _emailSender.SendEmailAsync(model.Email, model.Name, confirmationSubject, confirmationBody);
                
                if (confirmationSent)
                {
                    _logger.LogInformation("Confirmation email sent to sender {Email} for contact message ID={Id}", model.Email, contactMessage.Id);
                }
                else
                {
                    _logger.LogWarning("Failed to send confirmation email to sender {Email}", model.Email);
                }
            }
            catch (Exception emailEx)
            {
                // Don't fail the entire request if confirmation email fails
                _logger.LogError(emailEx, "Error sending confirmation email to {Email}", model.Email);
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

    private static string BuildAdminEmailBody(ContactFormModel model, string? sourceIp, string? userAgent, int messageId)
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

    private static string BuildSenderConfirmationEmail(ContactFormModel model)
    {
        return $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%); color: white; padding: 30px; text-align: center; border-radius: 5px 5px 0 0; }}
                    .header h1 {{ margin: 0; font-size: 24px; }}
                    .content {{ background: #f9f9f9; padding: 30px; border: 1px solid #ddd; border-radius: 0 0 5px 5px; }}
                    .message-summary {{ background: #fff; padding: 20px; margin: 20px 0; border-left: 4px solid #3b82f6; }}
                    .label {{ font-weight: bold; color: #555; }}
                    .value {{ color: #333; margin-bottom: 10px; }}
                    .next-steps {{ background: #fff; padding: 20px; margin: 20px 0; border-radius: 5px; }}
                    .next-steps h3 {{ color: #3b82f6; margin-top: 0; }}
                    .next-steps ul {{ padding-left: 20px; }}
                    .next-steps li {{ margin-bottom: 8px; }}
                    .footer {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; text-align: center; font-size: 12px; color: #666; }}
                    .contact-info {{ background: #fff; padding: 15px; margin: 15px 0; border-radius: 5px; }}
                    .contact-info a {{ color: #3b82f6; text-decoration: none; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>✓ Message Received</h1>
                        <p style='margin: 10px 0 0 0; font-size: 14px;'>Thank you for contacting Maruti Makwana Training</p>
                    </div>
                    <div class='content'>
                        <p>Hi <strong>{System.Web.HttpUtility.HtmlEncode(model.Name)}</strong>,</p>
                        
                        <p>Thank you for reaching out! I've received your message and will review it carefully.</p>
                        
                        <div class='message-summary'>
                            <h3 style='margin-top: 0; color: #3b82f6;'>Your Message Summary</h3>
                            <div class='value'>
                                <span class='label'>Subject:</span><br>
                                {System.Web.HttpUtility.HtmlEncode(model.Subject)}
                            </div>
                            <div class='value'>
                                <span class='label'>Message:</span><br>
                                <div style='white-space: pre-wrap;'>{System.Web.HttpUtility.HtmlEncode(model.Message)}</div>
                            </div>
                            {(!string.IsNullOrEmpty(model.Company) ? $@"
                            <div class='value'>
                                <span class='label'>Company:</span><br>
                                {System.Web.HttpUtility.HtmlEncode(model.Company)}
                            </div>" : "")}
                        </div>
                        
                        <div class='next-steps'>
                            <h3>What Happens Next?</h3>
                            <ul>
                                <li><strong>Response Time:</strong> I typically respond within 24-48 hours during business days</li>
                                <li><strong>Email Reply:</strong> I'll send a detailed response to <strong>{System.Web.HttpUtility.HtmlEncode(model.Email)}</strong></li>
                                {(!string.IsNullOrEmpty(model.Phone) ? $@"<li><strong>Phone Contact:</strong> I may also call you at {System.Web.HttpUtility.HtmlEncode(model.Phone)} if needed</li>" : "")}
                                <li><strong>Quick Questions:</strong> For urgent matters, feel free to reach out via WhatsApp or call directly</li>
                            </ul>
                        </div>
                        
                        <div class='contact-info'>
                            <p style='margin: 0 0 10px 0;'><strong>Meanwhile, you can:</strong></p>
                            <p style='margin: 5px 0;'>📧 <a href='mailto:maruti_makwana@hotmail.com'>Email me directly</a></p>
                            <p style='margin: 5px 0;'>💬 <a href='https://wa.me/919998114148' target='_blank'>Chat on WhatsApp</a></p>
                            <p style='margin: 5px 0;'>🌐 <a href='https://marutimakwana.com' target='_blank'>Visit my website</a></p>
                            <p style='margin: 5px 0;'>📅 <a href='https://calendly.com/mitg7805/30min' target='_blank'>Book a meeting directly</a></p>
                        </div>
                        
                        <p style='margin-top: 20px;'>Looking forward to connecting with you!</p>
                        
                        <p><strong>Best regards,</strong><br>
                        Maruti Makwana<br>
                        <em>Microsoft Certified Trainer (MCT)</em><br>
                        Azure Cloud & AI Corporate Training</p>
                    </div>
                    <div class='footer'>
                        <p>This is an automated confirmation. Please do not reply to this email.</p>
                        <p>© {DateTime.Now.Year} Maruti Makwana Training Portal. All rights reserved.</p>
                    </div>
                </div>
            </body>
            </html>
        ";
    }
}
