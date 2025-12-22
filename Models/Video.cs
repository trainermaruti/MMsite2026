using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MarutiTrainingPortal.Models
{
    /// <summary>
    /// Represents a video in the public video library.
    /// Newest videos are displayed first (PublishDate DESC).
    /// </summary>
    public class Video
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Video URL is required")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        [Display(Name = "YouTube Video URL")]
        public string VideoUrl { get; set; } = string.Empty;

        [Display(Name = "Thumbnail URL")]
        public string? ThumbnailUrl { get; set; }

        [Required(ErrorMessage = "Publish Date is required")]
        [Display(Name = "Publish Date")]
        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string? Category { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Extracts the YouTube Video ID from various URL formats.
        /// </summary>
        public string GetVideoId()
        {
            if (string.IsNullOrWhiteSpace(VideoUrl)) return string.Empty;

            try
            {
                var uri = new Uri(VideoUrl);
                if (uri.Host.Contains("youtu.be"))
                {
                    return uri.AbsolutePath.TrimStart('/');
                }
                
                var query = HttpUtility.ParseQueryString(uri.Query);
                return query["v"] ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Generates a high-quality YouTube thumbnail URL.
        /// </summary>
        public string GenerateThumbnailUrl()
        {
            var videoId = GetVideoId();
            return !string.IsNullOrEmpty(videoId) 
                ? $"https://img.youtube.com/vi/{videoId}/mqdefault.jpg" 
                : string.Empty;
        }

        /// <summary>
        /// Gets the Video ID for use in YouTube embed URLs.
        /// </summary>
        public string GetYouTubeEmbedId()
        {
            return GetVideoId();
        }
    }
}
