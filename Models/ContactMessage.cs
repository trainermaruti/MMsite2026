using System.ComponentModel.DataAnnotations;

namespace MarutiTrainingPortal.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
        public string Email { get; set; } = string.Empty;
        
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? PhoneNumber { get; set; }
        
        // Alias for PhoneNumber to maintain compatibility
        public string? Phone => PhoneNumber;
        
        [Required(ErrorMessage = "Subject is required")]
        [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters")]
        public string Subject { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Message is required")]
        [StringLength(2000, ErrorMessage = "Message cannot exceed 2000 characters")]
        public string Message { get; set; } = string.Empty;
        
        public bool IsRead { get; set; } = false;
        
        // For event registration interest
        public int? EventId { get; set; }
        
        // Lead management status
        [StringLength(50)]
        public string Status { get; set; } = "New"; // New, Contacted, Qualified, Converted, Closed
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        
        // Navigation property
        public TrainingEvent? Event { get; set; }
    }
}
