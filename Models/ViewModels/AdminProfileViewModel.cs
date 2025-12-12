using System.ComponentModel.DataAnnotations;

namespace MarutiTrainingPortal.Models.ViewModels
{
    public class AdminProfileViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Professional Title is required")]
        [StringLength(150, ErrorMessage = "Title cannot exceed 150 characters")]
        [Display(Name = "Professional Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bio is required")]
        [StringLength(2000, ErrorMessage = "Bio cannot exceed 2000 characters")]
        [Display(Name = "Short Bio")]
        public string Bio { get; set; } = string.Empty;

        [Required(ErrorMessage = "Expertise is required")]
        [Display(Name = "Expertise/Skills (comma-separated)")]
        public string Expertise { get; set; } = string.Empty;

        [Display(Name = "Certifications & Achievements")]
        [StringLength(1000)]
        public string? CertificationsAndAchievements { get; set; }

        [Display(Name = "Profile Image URL")]
        public string? ProfileImageUrl { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Public Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Phone(ErrorMessage = "Invalid WhatsApp number")]
        [Display(Name = "WhatsApp Number")]
        public string? WhatsAppNumber { get; set; }

        [Url(ErrorMessage = "Invalid LinkedIn URL")]
        [Display(Name = "LinkedIn Profile URL")]
        public string? LinkedInUrl { get; set; }

        [Url(ErrorMessage = "Invalid Instagram URL")]
        [Display(Name = "Instagram Profile URL")]
        public string? InstagramUrl { get; set; }

        [Url(ErrorMessage = "Invalid YouTube URL")]
        [Display(Name = "YouTube Channel URL")]
        public string? YouTubeUrl { get; set; }

        [Url(ErrorMessage = "Invalid SkillTech URL")]
        [Display(Name = "SkillTech Website URL")]
        public string? SkillTechUrl { get; set; }

        [Url(ErrorMessage = "Invalid Twitter URL")]
        [Display(Name = "Twitter/X Profile URL")]
        public string? TwitterUrl { get; set; }

        [Url(ErrorMessage = "Invalid GitHub URL")]
        [Display(Name = "GitHub Profile URL")]
        public string? GitHubUrl { get; set; }

        public DateTime UpdatedDate { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Current password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, ErrorMessage = "Password must be at least {2} characters long", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
            ErrorMessage = "Password must contain uppercase, lowercase, number and special character")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
