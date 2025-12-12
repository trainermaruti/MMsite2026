// Replace [PROJECT NAMESPACE] with your real namespace
#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MarutiTrainingPortal.ViewComponents
{
    /// <summary>
    /// Hero section with kinetic typography animation.
    /// Progressive enhancement: renders semantic HTML, JS adds kinetic effects.
    /// </summary>
    [ViewComponent(Name = "Hero")]
    public class HeroViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;

        public HeroViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IViewComponentResult Invoke()
        {
            // Check if animations are enabled globally
            var enableAnimations = _configuration.GetValue<bool>("EnableAnimations", true);
            
            var model = new HeroViewModel
            {
                Headline = "Azure & AI Training for Enterprise Teams",
                Subline = "Practical workshops and on-demand courses that upskill engineering teams",
                CtaPrimaryText = "Book a Workshop",
                CtaPrimaryUrl = "/Contact",
                CtaSecondaryText = "See Courses",
                CtaSecondaryUrl = "/Courses",
                EnableAnimations = enableAnimations
            };

            return View(model);
        }
    }

    public class HeroViewModel
    {
        public string Headline { get; set; } = string.Empty;
        public string Subline { get; set; } = string.Empty;
        public string CtaPrimaryText { get; set; } = string.Empty;
        public string CtaPrimaryUrl { get; set; } = string.Empty;
        public string CtaSecondaryText { get; set; } = string.Empty;
        public string CtaSecondaryUrl { get; set; } = string.Empty;
        public bool EnableAnimations { get; set; }
    }
}
