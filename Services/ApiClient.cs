// Replace [PROJECT NAMESPACE] with your real namespace
#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MarutiTrainingPortal.Models;

namespace MarutiTrainingPortal.Services
{
    /// <summary>
    /// API client for fetching public portfolio data.
    /// Falls back to local fixtures if API is unreachable.
    /// </summary>
    public class ApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiClient> _logger;
        private readonly string _fixturesPath;

        public ApiClient(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<ApiClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            _fixturesPath = Path.Combine(Directory.GetCurrentDirectory(), "data", "fixtures");
        }

        /// <summary>
        /// Fetches About tiles from API or fixture fallback.
        /// </summary>
        public async Task<List<AboutTile>> GetAboutTilesAsync()
        {
            return await FetchOrFallback<List<AboutTile>>(
                "about/tiles",
                "about-tiles.json"
            );
        }

        /// <summary>
        /// Fetches featured trainings from API or fixture fallback.
        /// </summary>
        public async Task<List<TrainingItem>> GetFeaturedTrainingsAsync()
        {
            return await FetchOrFallback<List<TrainingItem>>(
                "trainings/featured",
                "snake-trainings.json"
            );
        }

        /// <summary>
        /// Fetches projects from API or fixture fallback.
        /// </summary>
        public async Task<List<Project>> GetProjectsAsync()
        {
            return await FetchOrFallback<List<Project>>(
                "projects",
                "projects.json"
            );
        }

        /// <summary>
        /// Generic fetch with automatic fallback to fixture file.
        /// </summary>
        private async Task<T> FetchOrFallback<T>(string apiPath, string fixtureFileName) where T : new()
        {
            var apiBaseUrl = _configuration["PUBLIC_API_BASE_URL"];
            
            // Try API first if configured
            if (!string.IsNullOrEmpty(apiBaseUrl))
            {
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    client.Timeout = TimeSpan.FromSeconds(5); // Fast timeout for resilience
                    
                    var response = await client.GetAsync($"{apiBaseUrl.TrimEnd('/')}/{apiPath}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var data = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        
                        if (data != null)
                        {
                            _logger.LogInformation("Successfully fetched {Path} from API", apiPath);
                            return data;
                        }
                    }
                    
                    _logger.LogWarning("API returned non-success status for {Path}: {Status}", 
                        apiPath, response.StatusCode);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to fetch {Path} from API, falling back to fixtures", apiPath);
                }
            }

            // Fallback to fixture
            return await LoadFixture<T>(fixtureFileName);
        }

        /// <summary>
        /// Loads data from local JSON fixture file.
        /// </summary>
        private async Task<T> LoadFixture<T>(string fileName) where T : new()
        {
            var filePath = Path.Combine(_fixturesPath, fileName);
            
            if (!File.Exists(filePath))
            {
                _logger.LogError("Fixture file not found: {Path}", filePath);
                return new T();
            }

            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                var data = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                _logger.LogInformation("Loaded fixture data from {FileName}", fileName);
                return data ?? new T();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load fixture {FileName}", fileName);
                return new T();
            }
        }
    }
}
