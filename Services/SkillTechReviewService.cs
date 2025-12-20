using System.Text.Json;
using MarutiTrainingPortal.Models;

namespace MarutiTrainingPortal.Services
{
    /// <summary>
    /// Service to fetch student reviews from SkillTech.club or fallback to local data
    /// Supports automatic rotation and caching
    /// </summary>
    public interface ISkillTechReviewService
    {
        Task<List<StudentReview>> GetFeaturedReviewsAsync(int count = 3);
        Task<List<StudentReview>> GetAllReviewsAsync();
        Task RefreshCacheAsync();
    }

    public class SkillTechReviewService : ISkillTechReviewService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SkillTechReviewService> _logger;
        private static List<StudentReview>? _cachedReviews;
        private static DateTime _cacheExpiry = DateTime.MinValue;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);
        private static readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1, 1);

        public SkillTechReviewService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<SkillTechReviewService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Gets featured reviews (typically for homepage display)
        /// Automatically rotates reviews on each request for variety
        /// </summary>
        public async Task<List<StudentReview>> GetFeaturedReviewsAsync(int count = 3)
        {
            var allReviews = await GetAllReviewsAsync();
            
            // Shuffle reviews for variety (different reviews each time)
            var random = new Random();
            var shuffled = allReviews
                .Where(r => r.IsFeatured || r.IsVerified)
                .OrderBy(_ => random.Next())
                .Take(count)
                .ToList();

            _logger.LogInformation("📋 Serving {Count} featured reviews (from {Total} total)", 
                shuffled.Count, allReviews.Count);

            return shuffled;
        }

        /// <summary>
        /// Gets all reviews with caching
        /// </summary>
        public async Task<List<StudentReview>> GetAllReviewsAsync()
        {
            await _cacheLock.WaitAsync();
            try
            {
                // Return cached if still valid
                if (_cachedReviews != null && DateTime.UtcNow < _cacheExpiry)
                {
                    _logger.LogDebug("✅ Returning cached reviews ({Count} items)", _cachedReviews.Count);
                    return new List<StudentReview>(_cachedReviews);
                }

                // Try to fetch from SkillTech.club API
                var reviews = await TryFetchFromApiAsync();
                
                // Fallback to local reviews if API fails
                if (reviews == null || reviews.Count == 0)
                {
                    _logger.LogWarning("⚠️  API returned no reviews, using fallback data");
                    reviews = GetFallbackReviews();
                }

                // Update cache
                _cachedReviews = reviews;
                _cacheExpiry = DateTime.UtcNow.Add(CacheDuration);
                
                _logger.LogInformation("✅ Loaded {Count} reviews (cache expires at {Expiry})", 
                    reviews.Count, _cacheExpiry);

                return new List<StudentReview>(reviews);
            }
            finally
            {
                _cacheLock.Release();
            }
        }

        /// <summary>
        /// Manually refresh the cache (useful for admin actions)
        /// </summary>
        public async Task RefreshCacheAsync()
        {
            _logger.LogInformation("🔄 Manually refreshing review cache");
            _cachedReviews = null;
            _cacheExpiry = DateTime.MinValue;
            await GetAllReviewsAsync();
        }

        /// <summary>
        /// Attempts to fetch reviews from SkillTech.club API
        /// </summary>
        private async Task<List<StudentReview>?> TryFetchFromApiAsync()
        {
            var apiUrl = _configuration["SkillTech:ReviewApiUrl"];
            
            if (string.IsNullOrEmpty(apiUrl))
            {
                _logger.LogDebug("⚙️  No SkillTech API URL configured, using fallback");
                return null;
            }

            try
            {
                _logger.LogInformation("🌐 Fetching reviews from SkillTech API: {Url}", apiUrl);
                
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(10);

                var response = await client.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("⚠️  SkillTech API returned status {Status}", response.StatusCode);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var reviews = JsonSerializer.Deserialize<List<StudentReview>>(json, options);

                if (reviews != null && reviews.Any())
                {
                    _logger.LogInformation("✅ Successfully fetched {Count} reviews from SkillTech API", reviews.Count);
                    return reviews;
                }

                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "❌ HTTP error fetching reviews from SkillTech: {Message}", ex.Message);
                return null;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "⏱️  Timeout fetching reviews from SkillTech");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unexpected error fetching reviews: {Message}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Fallback reviews if API is unavailable
        /// These are the current hardcoded reviews from your site
        /// </summary>
        private List<StudentReview> GetFallbackReviews()
        {
            return new List<StudentReview>
            {
                new StudentReview
                {
                    Id = 1,
                    StudentName = "Ashek Rasul",
                    JobTitle = "Lead Product Designer",
                    Company = "",
                    ReviewText = "Maruti is one of the best trainers that I have seen. He is an excellent speaker and always well prepared with course content. He explains each topic very well and addresses each individual's questions.",
                    Rating = 5,
                    IsFeatured = true,
                    IsVerified = true
                },
                new StudentReview
                {
                    Id = 2,
                    StudentName = "Shubham Arya",
                    JobTitle = "BI Analyst",
                    Company = "IBM",
                    ReviewText = "Thanks for the Azure Kubernetes training. One point I liked most is that you made sure people with different knowledge levels are able to follow the training. Every minute was worth it!",
                    Rating = 5,
                    IsFeatured = true,
                    IsVerified = true
                },
                new StudentReview
                {
                    Id = 3,
                    StudentName = "Sneha K",
                    JobTitle = "BI Analyst",
                    Company = "IBM",
                    ReviewText = "Hands down the best training session I have attended. Maruti makes sure you understand the concepts. To the point, focused, and both conceptual and practical way of learning.",
                    Rating = 5,
                    IsFeatured = true,
                    IsVerified = true
                },
                new StudentReview
                {
                    Id = 4,
                    StudentName = "Priya Sharma",
                    JobTitle = "Cloud Engineer",
                    Company = "TCS",
                    ReviewText = "Exceptional Azure training! The hands-on labs and real-world scenarios made learning so effective. Highly recommend Maruti's courses.",
                    Rating = 5,
                    IsFeatured = true,
                    IsVerified = true
                },
                new StudentReview
                {
                    Id = 5,
                    StudentName = "Rajesh Kumar",
                    JobTitle = "DevOps Engineer",
                    Company = "Infosys",
                    ReviewText = "Best investment in my career! The AZ-400 training was comprehensive and helped me clear the certification on first attempt.",
                    Rating = 5,
                    IsFeatured = true,
                    IsVerified = true
                },
                new StudentReview
                {
                    Id = 6,
                    StudentName = "Anita Desai",
                    JobTitle = "Solutions Architect",
                    Company = "Wipro",
                    ReviewText = "Maruti's teaching style is engaging and practical. Complex Azure concepts were explained in simple terms. Excellent trainer!",
                    Rating = 5,
                    IsFeatured = true,
                    IsVerified = true
                }
            };
        }
    }
}
