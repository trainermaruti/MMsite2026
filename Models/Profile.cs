namespace MarutiTrainingPortal.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Title { get; set; }
        public string Bio { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string WhatsAppNumber { get; set; }
        public string Expertise { get; set; }
        public int TotalTrainingsDone { get; set; }
        public int TotalStudents { get; set; }
        public string LinkedInUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string YouTubeUrl { get; set; }
        public string SkillTechUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string GitHubUrl { get; set; }
        public string CertificationsAndAchievements { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
