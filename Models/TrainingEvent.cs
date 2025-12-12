using System.ComponentModel.DataAnnotations;

namespace MarutiTrainingPortal.Models
{
    public class TrainingEvent
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Summary { get; set; }
        
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        [StringLength(500, ErrorMessage = "Location cannot exceed 500 characters")]
        public string? Location { get; set; }
        
        [Required(ErrorMessage = "Event type is required")]
        [StringLength(100, ErrorMessage = "Event type cannot exceed 100 characters")]
        public string EventType { get; set; } = string.Empty;

        public bool IsOnline { get; set; } = false;

        [StringLength(100)]
        public string? TimeZone { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Draft"; // Draft, Upcoming, Open, Closed
        
        public int? MaxParticipants { get; set; }
        
        public int RegisteredParticipants { get; set; } = 0;
        
        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(500, ErrorMessage = "Registration link cannot exceed 500 characters")]
        public string? RegistrationLink { get; set; }
        
        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        public string? ImageUrl { get; set; }

        [StringLength(1000)]
        public string? BannerUrl { get; set; }
        
        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(500, ErrorMessage = "SkillTech URL cannot exceed 500 characters")]
        public string? SkillTechUrl { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Computed properties
        public bool IsFull => MaxParticipants.HasValue && RegisteredParticipants >= MaxParticipants.Value;
        public int AvailableSlots => MaxParticipants.HasValue ? Math.Max(0, MaxParticipants.Value - RegisteredParticipants) : int.MaxValue;
        public decimal CapacityPercentage => MaxParticipants.HasValue && MaxParticipants.Value > 0 
            ? (decimal)RegisteredParticipants / MaxParticipants.Value * 100 
            : 0;
    }
}
