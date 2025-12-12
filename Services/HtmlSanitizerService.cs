using Ganss.Xss;

namespace MarutiTrainingPortal.Services
{
    /// <summary>
    /// Wrapper service for HTML sanitization using Ganss.XSS (free library).
    /// Prevents XSS attacks by stripping dangerous HTML/JavaScript from user input.
    /// </summary>
    public interface IHtmlSanitizerService
    {
        string Sanitize(string html);
    }

    public class HtmlSanitizerService : IHtmlSanitizerService
    {
        private readonly HtmlSanitizer _sanitizer;

        public HtmlSanitizerService()
        {
            _sanitizer = new HtmlSanitizer();
            
            // Configure allowed tags (customize as needed)
            _sanitizer.AllowedTags.Add("p");
            _sanitizer.AllowedTags.Add("br");
            _sanitizer.AllowedTags.Add("strong");
            _sanitizer.AllowedTags.Add("em");
            _sanitizer.AllowedTags.Add("u");
            _sanitizer.AllowedTags.Add("h1");
            _sanitizer.AllowedTags.Add("h2");
            _sanitizer.AllowedTags.Add("h3");
            _sanitizer.AllowedTags.Add("ul");
            _sanitizer.AllowedTags.Add("ol");
            _sanitizer.AllowedTags.Add("li");
            _sanitizer.AllowedTags.Add("a");
            
            // Remove dangerous attributes
            _sanitizer.AllowedAttributes.Clear();
            _sanitizer.AllowedAttributes.Add("href");
            _sanitizer.AllowedAttributes.Add("title");
            _sanitizer.AllowedAttributes.Add("class");
        }

        public string Sanitize(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            return _sanitizer.Sanitize(html);
        }
    }
}
