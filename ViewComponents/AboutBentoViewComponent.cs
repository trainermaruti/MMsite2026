using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using MarutiTrainingPortal.Models;

namespace MarutiTrainingPortal.ViewComponents;

public class AboutBentoViewComponent : ViewComponent
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private static readonly string[] AllowedCssClasses = new[]
    {
        "col-span-1", "col-span-2", "col-span-3", "col-span-4", "col-span-5", "col-span-6",
        "col-span-7", "col-span-8", "col-span-9", "col-span-10", "col-span-11", "col-span-12",
        "lg:col-span-1", "lg:col-span-2", "lg:col-span-3", "lg:col-span-4", "lg:col-span-5", "lg:col-span-6",
        "lg:col-span-7", "lg:col-span-8", "lg:col-span-9", "lg:col-span-10", "lg:col-span-11", "lg:col-span-12",
        "md:col-span-1", "md:col-span-2", "md:col-span-3", "md:col-span-4", "md:col-span-5", "md:col-span-6",
        "md:col-span-7", "md:col-span-8", "md:col-span-9", "md:col-span-10", "md:col-span-11", "md:col-span-12",
        "row-span-1", "row-span-2", "row-span-3", "row-span-4",
        "lg:row-span-1", "lg:row-span-2", "lg:row-span-3", "lg:row-span-4",
        "md:row-span-1", "md:row-span-2", "md:row-span-3", "md:row-span-4"
    };

    public AboutBentoViewComponent(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var tiles = await FetchTilesAsync();
        
        // Validate and sanitize CSS classes
        foreach (var tile in tiles)
        {
            tile.CssClasses = ValidateCssClasses(tile.CssClasses);
            // TODO: Sanitize tile.ContentHtml server-side using HtmlSanitizer (e.g., Ganss.Xss)
        }

        var model = new AboutViewModel { Tiles = tiles };
        return View(model);
    }

    private async Task<List<AboutTile>> FetchTilesAsync()
    {
        var baseUrl = _configuration["PUBLIC_API_BASE_URL"];
        
        if (!string.IsNullOrEmpty(baseUrl))
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{baseUrl}/api/public/about/tiles");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tiles = JsonSerializer.Deserialize<List<AboutTile>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    if (tiles != null)
                        return tiles;
                }
            }
            catch
            {
                // Fall back to fixture
            }
        }

        // Fallback to local fixture
        var fixturePath = Path.Combine(Directory.GetCurrentDirectory(), "data", "fixtures", "about-tiles.json");
        if (File.Exists(fixturePath))
        {
            var fixtureContent = await File.ReadAllTextAsync(fixturePath);
            var tiles = JsonSerializer.Deserialize<List<AboutTile>>(fixtureContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            return tiles ?? new List<AboutTile>();
        }

        return new List<AboutTile>();
    }

    private string ValidateCssClasses(string cssClasses)
    {
        if (string.IsNullOrWhiteSpace(cssClasses))
            return "col-span-12 lg:col-span-4";

        var classes = cssClasses.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var validatedClasses = new List<string>();

        foreach (var cls in classes)
        {
            if (IsAllowedCssClass(cls))
                validatedClasses.Add(cls);
        }

        return validatedClasses.Count > 0 
            ? string.Join(" ", validatedClasses) 
            : "col-span-12 lg:col-span-4";
    }

    private bool IsAllowedCssClass(string cssClass)
    {
        return Array.IndexOf(AllowedCssClasses, cssClass) >= 0;
    }
}
