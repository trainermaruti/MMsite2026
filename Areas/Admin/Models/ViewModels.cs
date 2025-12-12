using System.ComponentModel.DataAnnotations;

namespace MarutiTrainingPortal.Areas.Admin.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public int TotalTrainings { get; set; }
        public int TotalCourses { get; set; }
        public int TotalEvents { get; set; }
        public int TotalMessages { get; set; }

        public List<RecentMessageViewModel> RecentMessages { get; set; } = new();
        public List<UpcomingEventViewModel> UpcomingEvents { get; set; } = new();
    }

    public class RecentMessageViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class UpcomingEventViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public int RegisteredParticipants { get; set; }
        public int MaxParticipants { get; set; }
    }

    public class ChartDataViewModel
    {
        public List<string> Labels { get; set; } = new();
        public List<int> Data { get; set; } = new();
    }
}
