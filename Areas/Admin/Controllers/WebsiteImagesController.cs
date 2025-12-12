using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;

namespace MarutiTrainingPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class WebsiteImagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public WebsiteImagesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Admin/WebsiteImages
        public async Task<IActionResult> Index(string category = "All")
        {
            var query = _context.WebsiteImages.AsQueryable();

            if (category != "All" && !string.IsNullOrEmpty(category))
            {
                query = query.Where(i => i.Category == category);
            }

            var images = await query.OrderBy(i => i.Category).ThenBy(i => i.DisplayName).ToListAsync();
            
            ViewBag.Categories = await _context.WebsiteImages
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
                TempData["SuccessMessage"] = "Image deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ImageExists(int id)
        {
            return _context.WebsiteImages.Any(e => e.Id == id);
        }
    }
}
