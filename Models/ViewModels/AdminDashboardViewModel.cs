namespace MarutiTrainingPortal.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalTrainings { get; set; }
        public int TotalCourses { get; set; }
        public int TotalEvents { get; set; }
        public int TotalContactMessages { get; set; }
        public int UnreadContactMessages { get; set; }
        public int TotalStudents { get; set; }
        public int UpcomingEvents { get; set; }
        public List<ContactMessage> RecentMessages { get; set; } = new();
        public List<TrainingEvent> UpcomingEventsList { get; set; } = new();
    }
}
