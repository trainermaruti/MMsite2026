using MarutiTrainingPortal.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MarutiTrainingPortal.Services
{
    /// <summary>
    /// Service for retrieving and caching site statistics.
    /// Uses IMemoryCache for performance (1-hour cache duration).
    /// For production scale, consider Redis/DistributedCache.
    /// </summary>
    public class StatService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<StatService> _logger;
        private const string STATS_CACHE_KEY = "SiteStatistics";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

        public StatService(
            ApplicationDbContext context,
            IMemoryCache cache,
            ILogger<StatService> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<SiteStatistics> GetSiteStatisticsAsync()
        {
            // Try to get from cache first
            if (_cache.TryGetValue(STATS_CACHE_KEY, out SiteStatistics? cachedStats))
            {
                _logger.LogInformation("Returning cached site statistics");
                return cachedStats!;
            }

            // If not in cache, fetch from database
            _logger.LogInformation("Fetching fresh site statistics from database");
            
            var stats = new SiteStatistics
            {
                TotalTrainings = await _context.Trainings.CountAsync(),
                TotalCourses = await _context.Courses.CountAsync(),
                TotalEvents = await _context.TrainingEvents.CountAsync(),
                TotalStudents = await _context.Certificates
                    .Select(c => c.StudentEmail)
                    .Distinct()
                    .CountAsync(),
                TotalEnrollments = await _context.Courses.SumAsync(c => c.TotalEnrollments),
                TotalCertificatesIssued = await _context.Certificates.CountAsync(),
                TotalContactMessages = await _context.ContactMessages.CountAsync(),
                UpcomingEvents = await _context.TrainingEvents
                    .Where(e => e.StartDate > DateTime.UtcNow)
                    .CountAsync(),
                LastUpdated = DateTime.UtcNow
            };

            // Cache the results
            _cache.Set(STATS_CACHE_KEY, stats, CacheDuration);
            _logger.LogInformation("Cached site statistics for {Duration}", CacheDuration);

            return stats;
        }

        public void InvalidateCache()
        {
            _cache.Remove(STATS_CACHE_KEY);
            _logger.LogInformation("Site statistics cache invalidated");
        }
    }

    public class SiteStatistics
    {
        public int TotalTrainings { get; set; }
        public int TotalCourses { get; set; }
        public int TotalEvents { get; set; }
        public int TotalStudents { get; set; }
        public int TotalEnrollments { get; set; }
        public int TotalCertificatesIssued { get; set; }
        public int TotalContactMessages { get; set; }
        public int UpcomingEvents { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
