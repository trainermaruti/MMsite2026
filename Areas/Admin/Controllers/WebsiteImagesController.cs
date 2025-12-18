using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;
using MarutiTrainingPortal.Services;

namespace MarutiTrainingPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class WebsiteImagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IImageService _imageService;

        public WebsiteImagesController(ApplicationDbContext context, IWebHostEnvironment environment, IImageService imageService)
        {
            _context = context;
            _environment = environment;
            _imageService = imageService;
        }

        // GET: Admin/WebsiteImages
        public async Task<IActionResult> Index(string category = "All")
        {
            var query = _context.WebsiteImages.Where(i => !i.IsDeleted).AsQueryable();

            if (category != "All" && !string.IsNullOrEmpty(category))
            {
                query = query.Where(i => i.Category == category);
            }

            var images = await query.OrderBy(i => i.Category).ThenBy(i => i.DisplayName).ToListAsync();
            
            ViewBag.Categories = await _context.WebsiteImages
                .Where(i => !i.IsDeleted && !string.IsNullOrEmpty(i.Category))
                .Select(i => i.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
            
            ViewBag.SelectedCategory = category;
            
            return View(images);
        }

        // GET: Admin/WebsiteImages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/WebsiteImages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WebsiteImage image, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                    Directory.CreateDirectory(uploadsFolder);
                    
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    
                    image.ImageUrl = "/images/" + uniqueFileName;
                }

                image.CreatedDate = DateTime.UtcNow;
                _context.WebsiteImages.Add(image);
                await _context.SaveChangesAsync();
                _imageService.ClearCache();
                await SaveImagesToJsonAsync();
                TempData["SuccessMessage"] = "Image added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(image);
        }

        // GET: Admin/WebsiteImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.WebsiteImages.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            return View(image);
        }

        // POST: Admin/WebsiteImages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WebsiteImage image, IFormFile? imageFile)
        {
            if (id != image.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get existing image from database
                    var existingImage = await _context.WebsiteImages.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
                    
                    // Handle new file upload
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                        Directory.CreateDirectory(uploadsFolder);
                        
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        
                        image.ImageUrl = "/images/" + uniqueFileName;
                    }
                    else if (existingImage != null)
                    {
                        // Preserve existing image URL if no new file uploaded
                        image.ImageUrl = existingImage.ImageUrl;
                    }

                    image.UpdatedDate = DateTime.UtcNow;
                    _context.Update(image);
                    await _context.SaveChangesAsync();
                    _imageService.ClearCache();
                    await SaveImagesToJsonAsync();
                    TempData["SuccessMessage"] = "Image updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImageExists(image.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(image);
        }

        // GET: Admin/WebsiteImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.WebsiteImages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // POST: Admin/WebsiteImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.WebsiteImages.FindAsync(id);
            if (image != null)
            {
                image.IsDeleted = true;
                _context.Update(image);
                await _context.SaveChangesAsync();
                _imageService.ClearCache();
                await SaveImagesToJsonAsync();
                TempData["SuccessMessage"] = "Image deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ImageExists(int id)
        {
            return _context.WebsiteImages.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> SyncToJson()
        {
            try
            {
                await SaveImagesToJsonAsync();
                TempData["SuccessMessage"] = "Successfully synced all images to JSON file!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error syncing to JSON: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/WebsiteImages/ImportFromJson
        [HttpPost]
        public async Task<IActionResult> ImportFromJson()
        {
            try
            {
                var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "JsonData", "ImagesDatabase.json");
                
                if (!System.IO.File.Exists(jsonPath))
                {
                    TempData["ErrorMessage"] = "JSON file not found!";
                    return RedirectToAction(nameof(Index));
                }

                var jsonContent = await System.IO.File.ReadAllTextAsync(jsonPath);
                var images = System.Text.Json.JsonSerializer.Deserialize<List<WebsiteImage>>(jsonContent);

                if (images == null || !images.Any())
                {
                    TempData["ErrorMessage"] = "No images found in JSON file!";
                    return RedirectToAction(nameof(Index));
                }

                // Get all valid image keys from JSON
                var validImageKeys = images.Select(i => i.ImageKey).ToHashSet();

                // Delete all images not in the JSON file
                var imagesToDelete = await _context.WebsiteImages
                    .Where(i => !validImageKeys.Contains(i.ImageKey))
                    .ToListAsync();

                _context.WebsiteImages.RemoveRange(imagesToDelete);

                int imported = 0;
                int updated = 0;

                foreach (var image in images)
                {
                    // Check if image already exists by ImageKey
                    var existing = await _context.WebsiteImages
                        .FirstOrDefaultAsync(i => i.ImageKey == image.ImageKey);

                    if (existing == null)
                    {
                        var newImage = new WebsiteImage
                        {
                            ImageKey = image.ImageKey,
                            DisplayName = image.DisplayName,
                            Description = image.Description,
                            ImageUrl = image.ImageUrl,
                            AltText = image.AltText,
                            Category = image.Category,
                            CreatedDate = DateTime.UtcNow,
                            IsDeleted = false
                        };
                        _context.WebsiteImages.Add(newImage);
                        imported++;
                    }
                    else
                    {
                        // Update existing image
                        existing.DisplayName = image.DisplayName;
                        existing.Description = image.Description;
                        existing.ImageUrl = image.ImageUrl;
                        existing.AltText = image.AltText;
                        existing.Category = image.Category;
                        existing.UpdatedDate = DateTime.UtcNow;
                        existing.IsDeleted = false;
                        updated++;
                    }
                }

                await _context.SaveChangesAsync();
                _imageService.ClearCache();

                TempData["SuccessMessage"] = $"Import complete! Imported: {imported}, Updated: {updated}, Removed: {imagesToDelete.Count}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error importing from JSON: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task SaveImagesToJsonAsync()
        {
            try
            {
                var images = await _context.WebsiteImages.OrderBy(i => i.Id).ToListAsync();
                var jsonData = images.Select(i => new
                {
                    i.ImageKey,
                    i.DisplayName,
                    i.Description,
                    i.ImageUrl,
                    i.AltText,
                    i.Category
                }).ToList();

                var json = System.Text.Json.JsonSerializer.Serialize(jsonData, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });

                var filePath = Path.Combine("JsonData", "ImagesDatabase.json");
                await System.IO.File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving images to JSON: {ex.Message}");
            }
        }
    }
}
