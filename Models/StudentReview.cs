namespace MarutiTrainingPortal.Models
{
    /// <summary>
    /// Student review/testimonial model
    /// Can be fetched from SkillTech.club API or stored locally
    /// </summary>
    public class StudentReview
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string ReviewText { get; set; } = string.Empty;
        public int Rating { get; set; } = 5; // 1-5 stars
        public string? AvatarUrl { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsVerified { get; set; } = true;
        public bool IsFeatured { get; set; } = false;
        
        /// <summary>
        /// Gets initials for avatar display (e.g., "John Doe" -> "JD")
        /// </summary>
        public string GetInitials()
        {
            if (string.IsNullOrWhiteSpace(StudentName))
                return "?";

            var parts = StudentName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return "?";
            
            if (parts.Length == 1)
                return parts[0][0].ToString().ToUpper();
            
            return $"{parts[0][0]}{parts[^1][0]}".ToUpper();
        }
        
        /// <summary>
        /// Gets star rating as HTML string (★★★★★)
        /// </summary>
        public string GetStarRating()
        {
            var fullStars = Math.Min(Rating, 5);
            return new string('★', fullStars);
        }
    }
}
