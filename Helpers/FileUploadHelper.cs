namespace MarutiTrainingPortal.Helpers
{
    public interface IFileUploadHelper
    {
        Task<string> UploadEventBannerAsync(IFormFile file);
        bool IsValidImage(IFormFile file);
    }

    public class FileUploadHelper : IFileUploadHelper
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileUploadHelper> _logger;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

        public FileUploadHelper(IWebHostEnvironment environment, ILogger<FileUploadHelper> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<string> UploadEventBannerAsync(IFormFile file)
        {
            if (!IsValidImage(file))
                throw new InvalidOperationException("Invalid image file");

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "events");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            _logger.LogInformation("Uploaded event banner: {FileName}", uniqueFileName);

            return $"/uploads/events/{uniqueFileName}";
        }

        public bool IsValidImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > MaxFileSize)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                return false;

            return true;
        }
    }
}
