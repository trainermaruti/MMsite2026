using System.ComponentModel.DataAnnotations;

namespace MarutiTrainingPortal.Models
{
    /// <summary>
    /// Represents a training completion certificate issued to a student.
    /// Certificates can be verified publicly via the /Verify endpoint.
    /// </summary>
    public class Certificate
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Unique certificate ID (e.g., "CERT-2024-001234")
        /// Used for public verification
        /// </summary>
        [Required]
        [StringLength(50)]
        public string CertificateId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Student name is required")]
        [StringLength(200, ErrorMessage = "Student name cannot exceed 200 characters")]
        public string StudentName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Student email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(200)]
        public string StudentEmail { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Course/Training title is required")]
        [StringLength(200)]
        public string CourseTitle { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? CourseCategory { get; set; }
        
        [Required(ErrorMessage = "Completion date is required")]
        public DateTime CompletionDate { get; set; }
        
        [Required(ErrorMessage = "Issue date is required")]
        public DateTime IssueDate { get; set; }
        
        [StringLength(200)]
        public string? Instructor { get; set; }
        
        [Range(0, 500, ErrorMessage = "Duration must be between 0 and 500 hours")]
        public int DurationHours { get; set; }
        
        [Range(0, 100, ErrorMessage = "Score must be between 0 and 100")]
        public decimal? Score { get; set; }
        
        [StringLength(50)]
        public string? Grade { get; set; } // e.g., "A+", "Pass", "Excellent"
        
        /// <summary>
        /// URL to certificate PDF or image
        /// </summary>
        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(500)]
        public string? CertificateUrl { get; set; }
        
        /// <summary>
        /// Additional verification notes visible to admin only
        /// </summary>
        [StringLength(1000)]
        public string? Notes { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        
        /// <summary>
        /// Set to true if certificate has been revoked
        /// </summary>
        public bool IsRevoked { get; set; } = false;
        
        public DateTime? RevokedDate { get; set; }
        
        [StringLength(500)]
        public string? RevocationReason { get; set; }
    }
}
