namespace MarutiTrainingPortal.Services
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ImageUploadService> _logger;
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public ImageUploadService(IWebHostEnvironment environment, ILogger<ImageUploadService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file provided");

            if (file.Length > MaxFileSize)
                throw new ArgumentException($"File size exceeds {MaxFileSize / 1024 / 1024}MB limit");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new ArgumentException($"Invalid file type. Allowed: {string.Join(", ", AllowedExtensions)}");

            try
            {
                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{extension}";
                var uploadPath = Path.Combine(_environment.WebRootPath, "images", folder);

                // Create directory if it doesn't exist
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative URL
                return $"/images/{folder}/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                throw new Exception("Failed to upload image", ex);
            }
        }

        public Task<bool> DeleteImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return Task.FromResult(false);

                var filePath = GetImagePath(imageUrl);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image: {ImageUrl}", imageUrl);
                return Task.FromResult(false);
            }
        }

        public string GetImagePath(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return string.Empty;

            // Remove leading slash if present
            var relativePath = imageUrl.TrimStart('/');
            return Path.Combine(_environment.WebRootPath, relativePath);
        }
    }
}
