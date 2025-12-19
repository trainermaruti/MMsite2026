using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;

namespace MarutiTrainingPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProfileDocumentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ProfileDocumentController> _logger;

        public ProfileDocumentController(
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            ILogger<ProfileDocumentController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        // GET: Admin/ProfileDocument
        public async Task<IActionResult> Index()
        {
            var documents = await _context.ProfileDocuments
                .Where(d => !d.IsDeleted)
                .OrderByDescending(d => d.CreatedDate)
                .ToListAsync();
            
            return View(documents);
        }

        // GET: Admin/ProfileDocument/Upload
        public IActionResult Upload()
        {
            return View(new ProfileDocument());
        }

        // POST: Admin/ProfileDocument/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(ProfileDocument model, IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Please select a PDF file to upload.");
                return View(model);
            }

            if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", "Only PDF files are allowed.");
                return View(model);
            }

            try
            {
                // Create documents directory if it doesn't exist
                var documentsPath = Path.Combine(_environment.WebRootPath, "documents");
                if (!Directory.Exists(documentsPath))
                {
                    Directory.CreateDirectory(documentsPath);
                }

                // Generate unique filename
                var fileName = $"profile_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
                var filePath = Path.Combine(documentsPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Create database entry
                var document = new ProfileDocument
                {
                    Title = model.Title,
                    Description = model.Description,
                    FilePath = $"/documents/{fileName}",
                    FileName = file.FileName,
                    FileSize = file.Length,
                    IsEnabled = model.IsEnabled,
                    CreatedDate = DateTime.UtcNow
                };

                _context.ProfileDocuments.Add(document);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Profile document uploaded successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile document");
                ModelState.AddModelError("", "An error occurred while uploading the file.");
                return View(model);
            }
        }

        // POST: Admin/ProfileDocument/Toggle/5
        [HttpPost]
        public async Task<IActionResult> Toggle(int id)
        {
            var document = await _context.ProfileDocuments.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            document.IsEnabled = !document.IsEnabled;
            document.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isEnabled = document.IsEnabled });
        }

        // POST: Admin/ProfileDocument/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var document = await _context.ProfileDocuments.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            // Soft delete
            document.IsDeleted = true;
            document.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Delete physical file
            try
            {
                var fullPath = Path.Combine(_environment.WebRootPath, document.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting physical file");
            }

            TempData["Success"] = "Profile document deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: View download statistics
        public async Task<IActionResult> Stats()
        {
            var documents = await _context.ProfileDocuments
                .Where(d => !d.IsDeleted)
                .OrderByDescending(d => d.DownloadCount)
                .ToListAsync();

            return View(documents);
        }
    }
}
