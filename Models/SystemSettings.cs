using System.ComponentModel.DataAnnotations;

namespace MarutiTrainingPortal.Models
{
    public class SystemSettings
    {
        [Key]
        public int Id { get; set; }

        // SEO Settings
        [Required]
        [StringLength(100)]
        public string SiteTitle { get; set; } = "Maruti Makwana Training Portal";

        [StringLength(500)]
        public string MetaDescription { get; set; } = "Professional Azure & AI Training by Maruti Makwana";

        [StringLength(300)]
        public string MetaKeywords { get; set; } = "Azure Training, AI Training, Cloud Computing, Corporate Training";

        public string? OgImageUrl { get; set; }
        public string? FaviconUrl { get; set; }

        // Email Notifications
        [EmailAddress]
        public string? ContactFormReceiverEmail { get; set; }

        [EmailAddress]
        public string? SecondaryNotificationEmail { get; set; }

        public bool EmailNotificationsEnabled { get; set; } = true;

        // Feature Toggles
        public bool MaintenanceMode { get; set; } = false;
        public bool ShowUpcomingEvents { get; set; } = true;
        public bool ShowCoursesSection { get; set; } = true;
        public bool ShowProfileSection { get; set; } = true;

        // Integrations (encrypted values stored separately)
        public bool SmtpConfigured { get; set; } = false;
        public bool SendGridConfigured { get; set; } = false;
        public bool AzureOpenAIConfigured { get; set; } = false;

        // System Info
        public string AppVersion { get; set; } = "1.0.0";
        public DateTime? LastBackupDate { get; set; }
        public string? DatabaseSize { get; set; }

        // Audit
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}
