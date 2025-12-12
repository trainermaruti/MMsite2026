// Replace [PROJECT NAMESPACE] with your real namespace
#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MarutiTrainingPortal.Models;
using MarutiTrainingPortal.Services;
using MarutiTrainingPortal.Helpers;
using System.Threading.Tasks;

namespace MarutiTrainingPortal.ViewComponents
{
    /// <summary>
    /// Bento Grid About section displaying tiles in a CSS Grid layout.
    /// Admin configures grid positioning via validated CssClasses.
    /// </summary>
    [ViewComponent(Name = "BentoGrid")]
    public class BentoGridViewComponent : ViewComponent
    {
        private readonly ApiClient _apiClient;
        private readonly ILogger<BentoGridViewComponent> _logger;

        public BentoGridViewComponent(ApiClient apiClient, ILogger<BentoGridViewComponent> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var tiles = await _apiClient.GetAboutTilesAsync();

            // SECURITY: Validate and sanitize CSS classes against whitelist
            foreach (var tile in tiles)
            {
                try
                {
                    tile.CssClasses = CssClassWhitelist.FilterCssClasses(tile.CssClasses);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Invalid CSS classes for tile {TileId}, filtering", tile.Id);
                    tile.CssClasses = string.Empty;
                }
            }

            return View(tiles);
        }
    }
}
