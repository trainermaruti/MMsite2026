// Replace [PROJECT NAMESPACE] with your real namespace
#nullable enable

using System.Collections.Generic;

namespace MarutiTrainingPortal.Models
{
    /// <summary>
    /// Aggregates all sections for the portfolio/home page view.
    /// </summary>
    public class PortfolioViewModel
    {
        public List<AboutTile> AboutTiles { get; set; } = new();
        
        public List<TrainingItem> FeaturedTrainings { get; set; } = new();
        
        public List<Project> Projects { get; set; } = new();
        
        public string HeroHeadline { get; set; } = "Azure & AI Training for Enterprise Teams";
        
        public string HeroSubline { get; set; } = "Practical workshops and on-demand courses that upskill engineering teams";
        
        public bool EnableAnimations { get; set; } = true;
    }
    
    /// <summary>
    /// Project/Portfolio item for showcasing work.
    /// </summary>
    public class Project
    {
        public int Id { get; set; }
        
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public string ImageUrl { get; set; } = string.Empty;
        
        public string AltText { get; set; } = string.Empty;
        
        public string? Url { get; set; }
        
        public string[] Tags { get; set; } = Array.Empty<string>();
        
        public bool IsFeatured { get; set; }
    }
}
