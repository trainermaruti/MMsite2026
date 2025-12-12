using System.ComponentModel.DataAnnotations;

namespace MarutiTrainingPortal.Models
{
    public class WebsiteImage
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        [Display(Name = "Image Key")]
        public string ImageKey { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        [StringLength(500)]
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = string.Empty;
        
        [StringLength(200)]
        [Display(Name = "Alt Text")]
        public string? AltText { get; set; }
        
        [StringLength(100)]
        public string? Category { get; set; } // Profile, About, Header, Hero, etc.
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedDate { get; set; }
        
        public bool IsDeleted { get; set; } = false;
    }
}
