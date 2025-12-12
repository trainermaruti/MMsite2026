using System.ComponentModel.DataAnnotations;

namespace MarutiTrainingPortal.Models
{
    public class TrainingEventRegistration
    {
        public int Id { get; set; }

        [Required]
        public int TrainingEventId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        [StringLength(1000)]
        public string? Notes { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation property
        public virtual TrainingEvent? TrainingEvent { get; set; }
    }
}
