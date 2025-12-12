using MarutiTrainingPortal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarutiTrainingPortal.Controllers
{
    /// <summary>
    /// Public certificate verification controller.
    /// No authentication required - allows anyone to verify certificates.
    /// Protected by rate limiting to prevent abuse.
    /// </summary>
    public class VerifyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VerifyController> _logger;

        public VerifyController(ApplicationDbContext context, ILogger<VerifyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Verify
        // Public page with certificate verification form
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: /Verify/Check
        // API endpoint for AJAX certificate lookup
        // Returns JSON with certificate details or not-found status
        [HttpPost]
        [Route("Verify/Check")]
        public async Task<IActionResult> Check([FromBody] VerifyRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.CertificateId))
                {
                    return BadRequest(new VerifyResponse
                    {
                        Success = false,
                        Message = "Certificate ID is required"
                    });
                }

                _logger.LogInformation("Certificate verification requested for ID: {CertificateId}", request.CertificateId);

                // Lookup certificate (ignore soft-deleted and revoked certificates)
                var certificate = await _context.Certificates
                    .IgnoreQueryFilters() // Need to explicitly check IsDeleted and IsRevoked
                    .FirstOrDefaultAsync(c => c.CertificateId == request.CertificateId.Trim());

                if (certificate == null || certificate.IsDeleted)
                {
                    _logger.LogWarning("Certificate not found: {CertificateId}", request.CertificateId);
                    
                    return NotFound(new VerifyResponse
                    {
                        Success = false,
                        Message = "Certificate not found. Please check the Certificate ID and try again."
                    });
                }

                if (certificate.IsRevoked)
                {
                    _logger.LogWarning("Revoked certificate accessed: {CertificateId}", request.CertificateId);
                    
                    return Ok(new VerifyResponse
                    {
                        Success = false,
                        Message = "This certificate has been revoked.",
                        CertificateData = new CertificateDto
                        {
                            CertificateId = certificate.CertificateId,
                            Status = "Revoked",
                            RevokedDate = certificate.RevokedDate,
                            RevocationReason = certificate.RevocationReason
                        }
                    });
                }

                // Return valid certificate data
                _logger.LogInformation("Valid certificate found: {CertificateId}", request.CertificateId);
                
                return Ok(new VerifyResponse
                {
                    Success = true,
                    Message = "Certificate verified successfully!",
                    CertificateData = new CertificateDto
                    {
                        CertificateId = certificate.CertificateId,
                        StudentName = certificate.StudentName,
                        CourseTitle = certificate.CourseTitle,
                        CourseCategory = certificate.CourseCategory,
                        CompletionDate = certificate.CompletionDate,
                        IssueDate = certificate.IssueDate,
                        Instructor = certificate.Instructor,
                        DurationHours = certificate.DurationHours,
                        Score = certificate.Score,
                        Grade = certificate.Grade,
                        Status = "Valid"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying certificate: {CertificateId}", request.CertificateId);
                
                return StatusCode(500, new VerifyResponse
                {
                    Success = false,
                    Message = "An error occurred while verifying the certificate. Please try again later."
                });
            }
        }
    }

    // Request/Response DTOs
    public class VerifyRequest
    {
        public string CertificateId { get; set; } = string.Empty;
    }

    public class VerifyResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public CertificateDto? CertificateData { get; set; }
    }

    public class CertificateDto
    {
        public string CertificateId { get; set; } = string.Empty;
        public string? StudentName { get; set; }
        public string? CourseTitle { get; set; }
        public string? CourseCategory { get; set; }
        public DateTime? CompletionDate { get; set; }
        public DateTime? IssueDate { get; set; }
        public string? Instructor { get; set; }
        public int? DurationHours { get; set; }
        public decimal? Score { get; set; }
        public string? Grade { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? RevokedDate { get; set; }
        public string? RevocationReason { get; set; }
    }
}
