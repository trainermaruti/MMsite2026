using System.ComponentModel.DataAnnotations;

namespace MarutiTrainingPortal.Models.ViewModels
{
    public class SystemSettingsViewModel
    {
        public int Id { get; set; }

        // SEO Settings
        [Required]
        [StringLength(100)]
        [Display(Name = "Site Title")]
        public string SiteTitle { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Meta Description")]
        public string? MetaDescription { get; set; }

        [StringLength(300)]
        [Display(Name = "Meta Keywords (comma-separated)")]
        public string? MetaKeywords { get; set; }

        [Display(Name = "OG Image URL")]
        public string? OgImageUrl { get; set; }

        [Display(Name = "Favicon URL")]
        public string? FaviconUrl { get; set; }

        // Email Notifications
        [EmailAddress]
        [Display(Name = "Contact Form Receiver Email")]
        public string? ContactFormReceiverEmail { get; set; }

        [EmailAddress]
        [Display(Name = "Secondary Notification Email")]
        public string? SecondaryNotificationEmail { get; set; }

        [Display(Name = "Enable Email Notifications")]
        public bool EmailNotificationsEnabled { get; set; }

        // Feature Toggles
        [Display(Name = "Maintenance Mode")]
        public bool MaintenanceMode { get; set; }

        [Display(Name = "Show Upcoming Events Section")]
        public bool ShowUpcomingEvents { get; set; }

        [Display(Name = "Show Courses Section")]
        public bool ShowCoursesSection { get; set; }

        [Display(Name = "Show Profile Section")]
        public bool ShowProfileSection { get; set; }

        // Integration Status (read-only in UI)
        public bool SmtpConfigured { get; set; }
        public bool SendGridConfigured { get; set; }
        public bool AzureOpenAIConfigured { get; set; }

        // System Info (read-only)
        public string AppVersion { get; set; } = "1.0.0";
        public DateTime? LastBackupDate { get; set; }
        public string? DatabaseSize { get; set; }

        public DateTime UpdatedDate { get; set; }
    }

    public class SmtpConfigViewModel
    {
        [Required]
        [Display(Name = "SMTP Host")]
        public string SmtpHost { get; set; } = string.Empty;

        [Required]
        [Range(1, 65535)]
        [Display(Name = "SMTP Port")]
        public int SmtpPort { get; set; } = 587;

        [Required]
        [Display(Name = "SMTP Username")]
        public string SmtpUsername { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "SMTP Password")]
        public string SmtpPassword { get; set; } = string.Empty;

        [Display(Name = "Enable SSL")]
        public bool EnableSsl { get; set; } = true;
    }

    public class ApiKeyViewModel
    {
        [Display(Name = "SendGrid API Key")]
        public string? SendGridApiKey { get; set; }

        [Display(Name = "Azure OpenAI API Key")]
        public string? AzureOpenAIApiKey { get; set; }

        [Display(Name = "Azure OpenAI Endpoint")]
        public string? AzureOpenAIEndpoint { get; set; }
    }
}
