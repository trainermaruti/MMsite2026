using System.ComponentModel.DataAnnotations;

namespace MarutiTrainingPortal.Models
{
    /// <summary>
    /// Audit log for tracking changes to contact messages/leads
    /// </summary>
    public class LeadAuditLog
    {
        public int Id { get; set; }
        
        [Required]
        public int ContactMessageId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty; // "StatusChanged", "Created", "Updated", "Deleted"
        
        [StringLength(50)]
        public string? OldValue { get; set; }
        
        [StringLength(50)]
        public string? NewValue { get; set; }
        
        [Required]
        [StringLength(200)]
        public string ChangedBy { get; set; } = string.Empty; // Username or email of admin
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        public ContactMessage? ContactMessage { get; set; }
    }
}
