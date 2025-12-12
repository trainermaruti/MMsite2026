// Replace [PROJECT NAMESPACE] with your real namespace
using System.Collections.Generic;
using System.Linq;

namespace MarutiTrainingPortal.Helpers
{
    /// <summary>
    /// Server-side CSS class validator to prevent layout injection attacks.
    /// Only allows curated Tailwind utility classes and component-specific tokens.
    /// </summary>
    public static class CssClassWhitelist
    {
        private static readonly HashSet<string> AllowedClasses = new()
        {
            // Grid positioning
            "col-span-1", "col-span-2", "col-span-3", "col-span-4", "col-span-5", "col-span-6",
            "col-span-7", "col-span-8", "col-span-9", "col-span-10", "col-span-11", "col-span-12",
            "row-span-1", "row-span-2", "row-span-3", "row-span-4", "row-span-5", "row-span-6",
            
            // Color themes
            "primary", "secondary", "accent", "success", "warning", "info", "danger",
            "bg-void-black", "bg-charcoal", "bg-accent", "bg-accent-glow",
            "text-accent", "text-accent-glow",
            
            // Display utilities
            "hidden", "block", "inline-block", "flex", "inline-flex", "grid",
            
            // Common spacing
            "p-4", "p-6", "p-8", "m-4", "m-6", "m-8",
            "gap-2", "gap-4", "gap-6", "gap-8",
            
            // Border radius
            "rounded", "rounded-md", "rounded-lg", "rounded-xl", "rounded-2xl", "rounded-full",
            
            // Typography
            "text-sm", "text-base", "text-lg", "text-xl", "text-2xl", "text-3xl",
            "font-normal", "font-medium", "font-semibold", "font-bold",
            
            // Component-specific
            "glass-card", "bento-tile", "snake-node", "hero-text", "timeline-dot"
        };

        /// <summary>
        /// Validates if a single CSS class is allowed.
        /// </summary>
        public static bool IsAllowedCssClass(string cssClass)
        {
            if (string.IsNullOrWhiteSpace(cssClass))
                return false;

            return AllowedClasses.Contains(cssClass.Trim());
        }

        /// <summary>
        /// Filters a space-separated string of CSS classes, returning only allowed ones.
        /// </summary>
        public static string FilterCssClasses(string? cssClasses)
        {
            if (string.IsNullOrWhiteSpace(cssClasses))
                return string.Empty;

            var classes = cssClasses.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var allowed = classes.Where(IsAllowedCssClass);
            
            return string.Join(" ", allowed);
        }

        /// <summary>
        /// Validates and returns sanitized CSS classes or throws if any are invalid.
        /// Use this when strict validation is required.
        /// </summary>
        public static string ValidateAndSanitizeCssClasses(string? cssClasses)
        {
            if (string.IsNullOrWhiteSpace(cssClasses))
                return string.Empty;

            var classes = cssClasses.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var invalid = classes.Where(c => !IsAllowedCssClass(c)).ToList();

            if (invalid.Any())
            {
                throw new ArgumentException(
                    $"Invalid CSS classes detected: {string.Join(", ", invalid)}. " +
                    "Only whitelisted Tailwind utilities are allowed."
                );
            }

            return string.Join(" ", classes);
        }
    }
}
