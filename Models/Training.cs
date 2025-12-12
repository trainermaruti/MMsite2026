using System.ComponentModel.DataAnnotations;

namespace MarutiTrainingPortal.Models
{
    public class Training
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Company is required")]
        [StringLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
        public string Company { get; set; } = string.Empty;
        
        public DateTime? DeliveryDate { get; set; }
        
        [StringLength(100, ErrorMessage = "Duration cannot exceed 100 characters")]
        public string? Duration { get; set; }
        
        [StringLength(1000, ErrorMessage = "Topics cannot exceed 1000 characters")]
        public string? Topics { get; set; }
        
        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        public string? ImageUrl { get; set; }
        
        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(500, ErrorMessage = "SkillTech URL cannot exceed 500 characters")]
        public string? SkillTechUrl { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
