using System.ComponentModel.DataAnnotations;

namespace MarutiTrainingPortal.Models
{
    public class ProfileDocument
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = "Professional Profile";
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string FileName { get; set; } = "profile.pdf";
        
        public long FileSize { get; set; }
        
        public bool IsEnabled { get; set; } = true;
        
        public int DownloadCount { get; set; } = 0;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedDate { get; set; }
        
        public bool IsDeleted { get; set; } = false;
        
        // Helper property for display
        public string FileSizeFormatted => FileSize > 1024 * 1024 
            ? $"{FileSize / (1024.0 * 1024.0):F2} MB" 
            : $"{FileSize / 1024.0:F2} KB";
    }
}
