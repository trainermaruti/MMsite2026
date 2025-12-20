using Microsoft.AspNetCore.Mvc;
using MarutiTrainingPortal.Services;

namespace MarutiTrainingPortal.ViewComponents
{
    /// <summary>
    /// View component to display student reviews dynamically
    /// Can be used on any page: @await Component.InvokeAsync("StudentReviews", new { count = 3 })
    /// </summary>
    [ViewComponent(Name = "StudentReviews")]
    public class StudentReviewsViewComponent : ViewComponent
    {
        private readonly ISkillTechReviewService _reviewService;
        private readonly ILogger<StudentReviewsViewComponent> _logger;

        public StudentReviewsViewComponent(
            ISkillTechReviewService reviewService,
            ILogger<StudentReviewsViewComponent> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the view component with specified number of reviews
        /// </summary>
        /// <param name="count">Number of reviews to display (default: 3)</param>
        /// <param name="shuffle">Whether to shuffle reviews for variety (default: true)</param>
        public async Task<IViewComponentResult> InvokeAsync(int count = 3, bool shuffle = true)
        {
            try
            {
                var reviews = await _reviewService.GetFeaturedReviewsAsync(count);
                
                if (reviews == null || !reviews.Any())
                {
                    _logger.LogWarning("⚠️  No reviews available to display");
                    return Content(string.Empty); // Return empty to avoid breaking page
                }

                _logger.LogInformation("📋 Displaying {Count} student reviews", reviews.Count);
                return View(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error loading student reviews");
                return Content(string.Empty); // Graceful degradation
            }
        }
    }
}
