namespace MarutiTrainingPortal.Models
{
    public class TrainingItem
    {
        public int Id { get; set; }
        public string Client { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
        public string ExcerptHtml { get; set; } = string.Empty;
        public string CssClasses { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }
    }
}
