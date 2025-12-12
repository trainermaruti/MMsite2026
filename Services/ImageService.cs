using MarutiTrainingPortal.Data;
using Microsoft.EntityFrameworkCore;

namespace MarutiTrainingPortal.Services
{
    public interface IImageService
    {
        Task<string?> GetImageUrlAsync(string imageKey);
        Task<Dictionary<string, string>> GetAllImageUrlsAsync();
    }

    public class ImageService : IImageService
    {
        private readonly ApplicationDbContext _context;
        private static Dictionary<string, string>? _imageCache;
        private static DateTime _cacheLastUpdated = DateTime.MinValue;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        public ImageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string?> GetImageUrlAsync(string imageKey)
        {
            await RefreshCacheIfNeeded();
            
            if (_imageCache != null && _imageCache.TryGetValue(imageKey, out var url))
            {
                return url;
            }
            
            // Fallback to default images if key not found
            return GetFallbackImage(imageKey);
        }

        private string GetFallbackImage(string imageKey)
        {
            // Return default images based on image key
            return imageKey switch
            {
                "profile_main" => "/images/44.png",
                "profile_hero" => "/images/22.png",
                "experience_badge" => "/images/experience-badge.png",
                _ => "/images/44.png" // Default fallback
            };
        }

        public async Task<Dictionary<string, string>> GetAllImageUrlsAsync()
        {
            await RefreshCacheIfNeeded();
            return _imageCache ?? new Dictionary<string, string>();
        }

        private async Task RefreshCacheIfNeeded()
        {
            if (_imageCache == null || DateTime.UtcNow - _cacheLastUpdated > CacheDuration)
            {
                _imageCache = await _context.WebsiteImages
                    .Where(i => !i.IsDeleted)
                    .ToDictionaryAsync(i => i.ImageKey, i => i.ImageUrl);
                
                _cacheLastUpdated = DateTime.UtcNow;
            }
        }
    }
}
