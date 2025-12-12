using System.Collections.Concurrent;

namespace MarutiTrainingPortal.Services
{
    /// <summary>
    /// In-memory rate limiter using sliding window algorithm (free, local).
    /// Production note: For multi-server deployments, use Redis-backed distributed cache.
    /// </summary>
    public interface IRateLimitService
    {
        bool IsAllowed(string identifier, int maxRequests = 5, TimeSpan? window = null);
    }

    public class RateLimitService : IRateLimitService
    {
        private readonly ConcurrentDictionary<string, Queue<DateTime>> _requestLog = new();
        private readonly TimeSpan _defaultWindow = TimeSpan.FromMinutes(5);

        public bool IsAllowed(string identifier, int maxRequests = 5, TimeSpan? window = null)
        {
            var windowSpan = window ?? _defaultWindow;
            var now = DateTime.UtcNow;
            
            var requests = _requestLog.GetOrAdd(identifier, _ => new Queue<DateTime>());

            lock (requests)
            {
                // Remove expired requests outside the window
                while (requests.Count > 0 && requests.Peek() < now - windowSpan)
                {
                    requests.Dequeue();
                }

                if (requests.Count >= maxRequests)
                {
                    return false; // Rate limit exceeded
                }

                requests.Enqueue(now);
                return true;
            }
        }
    }

    /// <summary>
    /// PRODUCTION UPGRADE (optional, requires Redis - free tier available on Azure/Render):
    /// Replace in-memory RateLimitService with Redis-backed distributed cache for multi-server deployments.
    /// 
    /// Install: dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
    /// 
    /// Configure in Program.cs:
    /// builder.Services.AddStackExchangeRedisCache(options =>
    /// {
    ///     options.Configuration = builder.Configuration["Redis:ConnectionString"];
    ///     options.InstanceName = "MarutiPortal_";
    /// });
    /// 
    /// Then implement DistributedRateLimitService using IDistributedCache.
    /// </summary>
}
