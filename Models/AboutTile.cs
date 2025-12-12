// Replace [PROJECT NAMESPACE] with your real namespace
#nullable enable

namespace MarutiTrainingPortal.Models
{
    /// <summary>
    /// Represents a single tile in the Bento Grid About section.
    /// Admin can configure grid placement via CssClasses (col-span-*, row-span-*).
    /// </summary>
    public class AboutTile
    {
        public int Id { get; set; }
        
        public string Title { get; set; } = string.Empty;
        
        public string Subtitle { get; set; } = string.Empty;
        
        /// <summary>
        /// HTML content for the tile body.
        /// SECURITY: Must be sanitized server-side before rendering.
        /// </summary>
        public string ContentHtml { get; set; } = string.Empty;
        
        public string? ImageUrl { get; set; }
        
        public string? AltText { get; set; }
        
        /// <summary>
        /// Space-separated CSS classes for grid positioning (e.g., "col-span-2 row-span-1").
        /// SECURITY: Must be validated against CssClassWhitelist before use.
        /// </summary>
        public string CssClasses { get; set; } = string.Empty;
        
        public string? IconClass { get; set; }
        
        public bool IsFeatured { get; set; }
        
        public int DisplayOrder { get; set; }
    }
}
