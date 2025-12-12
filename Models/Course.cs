using System.ComponentModel.DataAnnotations;

namespace MarutiTrainingPortal.Models
{
    public class Course
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Category is required")]
        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string Category { get; set; } = string.Empty;
        
        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(500, ErrorMessage = "Thumbnail URL cannot exceed 500 characters")]
        public string? ThumbnailUrl { get; set; }
        
        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(500, ErrorMessage = "Video URL cannot exceed 500 characters")]
        public string? VideoUrl { get; set; }
        
        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(500, ErrorMessage = "SkillTech URL cannot exceed 500 characters")]
        public string? SkillTechUrl { get; set; }
        
        [Range(1, 10000, ErrorMessage = "Duration must be between 1 and 10000 minutes")]
        public int DurationMinutes { get; set; }
        
        // Duration in seconds for precise HH:MM:SS display
        public int DurationSeconds { get; set; }
        
        [Required(ErrorMessage = "Level is required")]
        [StringLength(50, ErrorMessage = "Level cannot exceed 50 characters")]
        public string Level { get; set; } = string.Empty;
        
        [Range(0, 100000, ErrorMessage = "Price must be between 0 and 100000")]
        public decimal Price { get; set; }
        
        public int TotalEnrollments { get; set; } = 0;
        
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
        public double Rating { get; set; } = 0;
        
        public DateTime PublishedDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
