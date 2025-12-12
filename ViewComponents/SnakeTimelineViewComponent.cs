using Microsoft.AspNetCore.Mvc;
using MarutiTrainingPortal.Models;
using System.Text.Json;

namespace MarutiTrainingPortal.ViewComponents
{
    [ViewComponent(Name = "SnakeTimeline")]
    public class SnakeTimelineViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SnakeTimelineViewComponent> _logger;

        public SnakeTimelineViewComponent(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<SnakeTimelineViewComponent> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<TrainingItem> trainings = new();

            try
            {
                // Try to fetch from API first
                var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
                if (!string.IsNullOrEmpty(apiBaseUrl))
                {
                    var client = _httpClientFactory.CreateClient();
                    var response = await client.GetAsync($"{apiBaseUrl}/api/trainings/featured");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        trainings = JsonSerializer.Deserialize<List<TrainingItem>>(json, 
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to fetch trainings from API, falling back to fixture data");
            }

            // Fallback to fixture data if API fails or returns no data
            if (!trainings.Any())
            {
                try
                {
                    var fixturePath = Path.Combine(Directory.GetCurrentDirectory(), "data", "fixtures", "snake-trainings.json");
                    if (System.IO.File.Exists(fixturePath))
                    {
                        var json = await System.IO.File.ReadAllTextAsync(fixturePath);
                        trainings = JsonSerializer.Deserialize<List<TrainingItem>>(json,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load fixture data for Snake Timeline");
                }
            }

            // Filter featured and validate CSS classes
            trainings = trainings
                .Where(t => t.IsFeatured)
                .Select(t => 
                {
                    // Sanitize CSS classes (basic whitelist validation)
                    if (!string.IsNullOrEmpty(t.CssClasses))
                    {
                        var allowedClasses = new[] { "primary", "secondary", "accent", "success", "warning", "info" };
                        var classes = t.CssClasses.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        t.CssClasses = string.Join(" ", classes.Where(c => allowedClasses.Contains(c)));
                    }
                    return t;
                })
                .ToList();

            return View(trainings);
        }
    }
}
