using System.Collections.Concurrent;

namespace MarutiTrainingPortal.Middleware
{
    /// <summary>
    /// In-memory rate limiting middleware using sliding window algorithm.
    /// Protects contact form and certificate verification endpoints from abuse.
    /// 
    /// NOTE: For production scale with multiple servers, use Redis-based distributed cache.
    /// Example: services.AddStackExchangeRedisCache(options => { ... });
    /// </summary>
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitMiddleware> _logger;
        private readonly IConfiguration _configuration;

        // In-memory store: IP -> List of request timestamps
        // NOTE: For horizontal scaling, replace with Redis/DistributedCache
        private static readonly ConcurrentDictionary<string, List<DateTime>> _requestLog = new();

        public RateLimitMiddleware(
            RequestDelegate next,
            ILogger<RateLimitMiddleware> logger,
            IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? string.Empty;

            // Only apply rate limiting to specific endpoints
            if (ShouldRateLimit(path))
            {
                var clientIp = GetClientIp(context);
                var isAllowed = CheckRateLimit(clientIp, path);

                if (!isAllowed)
                {
                    _logger.LogWarning("Rate limit exceeded for IP: {ClientIp} on path: {Path}", clientIp, path);
                    
                    context.Response.StatusCode = 429; // Too Many Requests
                    context.Response.ContentType = "application/json";
                    
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "Rate limit exceeded",
                        message = "Too many requests. Please try again later.",
                        retryAfter = GetRetryAfterSeconds(path)
                    });
                    
                    return;
                }
            }

            await _next(context);
        }

        private bool ShouldRateLimit(string path)
        {
            // Apply rate limiting to these endpoints
            return path.Contains("/contact") ||
                   path.Contains("/verify/check") ||
                   path.Contains("/api/events/register");
        }

        private bool CheckRateLimit(string clientIp, string path)
        {
            var now = DateTime.UtcNow;
            var windowSeconds = GetWindowSeconds(path);
            var maxRequests = GetMaxRequests(path);

            // Get or create request log for this IP
            var requests = _requestLog.GetOrAdd(clientIp, _ => new List<DateTime>());

            lock (requests)
            {
                // Remove old requests outside the sliding window
                requests.RemoveAll(r => (now - r).TotalSeconds > windowSeconds);

                // Check if within limit
                if (requests.Count >= maxRequests)
                {
                    return false; // Rate limit exceeded
                }

                // Add current request
                requests.Add(now);
                return true;
            }
        }

        private string GetClientIp(HttpContext context)
        {
            // Try to get real IP from headers (if behind proxy/load balancer)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        private int GetWindowSeconds(string path)
        {
            // Contact form: 1 minute window
            if (path.Contains("/contact"))
                return _configuration.GetValue("RateLimit:Contact:WindowSeconds", 60);

            // Verify endpoint: 1 minute window
            if (path.Contains("/verify"))
                return _configuration.GetValue("RateLimit:Verify:WindowSeconds", 60);

            // Event registration: 5 minute window
            return _configuration.GetValue("RateLimit:Event:WindowSeconds", 300);
        }

        private int GetMaxRequests(string path)
        {
            // Contact form: 3 requests per window
            if (path.Contains("/contact"))
                return _configuration.GetValue("RateLimit:Contact:MaxRequests", 3);

            // Verify endpoint: 10 requests per window
            if (path.Contains("/verify"))
                return _configuration.GetValue("RateLimit:Verify:MaxRequests", 10);

            // Event registration: 5 requests per window
            return _configuration.GetValue("RateLimit:Event:MaxRequests", 5);
        }

        private int GetRetryAfterSeconds(string path)
        {
            return GetWindowSeconds(path);
        }

        /// <summary>
        /// Background cleanup task to prevent memory leaks.
        /// Call this periodically (e.g., via IHostedService).
        /// </summary>
        public static void CleanupOldEntries()
        {
            var cutoff = DateTime.UtcNow.AddHours(-1);
            
            foreach (var key in _requestLog.Keys.ToList())
            {
                if (_requestLog.TryGetValue(key, out var requests))
                {
                    lock (requests)
                    {
                        requests.RemoveAll(r => r < cutoff);
                        
                        // Remove empty entries
                        if (requests.Count == 0)
                        {
                            _requestLog.TryRemove(key, out _);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Background service to cleanup old rate limit entries.
    /// Runs every 30 minutes.
    /// </summary>
    public class RateLimitCleanupService : BackgroundService
    {
        private readonly ILogger<RateLimitCleanupService> _logger;

        public RateLimitCleanupService(ILogger<RateLimitCleanupService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Running rate limit cleanup");
                    RateLimitMiddleware.CleanupOldEntries();
                    await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during rate limit cleanup");
                }
            }
        }
    }
}
