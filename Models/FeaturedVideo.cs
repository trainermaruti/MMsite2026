using System.ComponentModel.DataAnnotations;

namespace MarutiTrainingPortal.Models
{
    public class FeaturedVideo
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        [Required]
        [StringLength(500)]
        [Display(Name = "YouTube URL")]
        public string YouTubeUrl { get; set; } = string.Empty;
        
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; } = 0;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedDate { get; set; }
        
        public bool IsDeleted { get; set; } = false;
        
        // Helper method to extract YouTube video ID from URL
        public string GetYouTubeEmbedId()
        {
            if (string.IsNullOrEmpty(YouTubeUrl))
                return string.Empty;
            
            try
            {
                var uri = new Uri(YouTubeUrl);
                
                // Handle different YouTube URL formats
                if (uri.Host.Contains("youtu.be"))
                {
                    return uri.AbsolutePath.TrimStart('/');
                }
                else if (uri.Host.Contains("youtube.com"))
                {
                    var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    return query["v"] ?? string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
            
            return string.Empty;
        }
    }
}
