using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarutiTrainingPortal.Data;

namespace MarutiTrainingPortal.Controllers
{
    public class DocumentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DocumentController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: /profile.pdf - Clean URL for profile download
        [HttpGet("/profile.pdf")]
        public async Task<IActionResult> DownloadProfile()
        {
            // Get the active profile document
            var document = await _context.ProfileDocuments
                .Where(d => d.IsEnabled && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedDate)
                .FirstOrDefaultAsync();

            if (document == null)
            {
                return NotFound("Profile document not available.");
            }

            // Increment download count
            document.DownloadCount++;
            await _context.SaveChangesAsync();

            // Get file path
            var filePath = Path.Combine(_environment.WebRootPath, document.FilePath.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            // Return file as download
            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf", "Maruti_Makwana_Profile.pdf");
        }

        // GET: /api/profile-document/status - Check if document is available
        [HttpGet("/api/profile-document/status")]
        public async Task<IActionResult> GetStatus()
        {
            var document = await _context.ProfileDocuments
                .Where(d => d.IsEnabled && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedDate)
                .FirstOrDefaultAsync();

            return Json(new
            {
                available = document != null,
                title = document?.Title,
                description = document?.Description,
                fileSize = document?.FileSizeFormatted,
                downloadUrl = "/profile.pdf"
            });
        }
    }
}
