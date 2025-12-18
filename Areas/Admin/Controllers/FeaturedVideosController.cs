using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;

namespace MarutiTrainingPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class FeaturedVideosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeaturedVideosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/FeaturedVideos
        public async Task<IActionResult> Index()
        {
            var videos = await _context.FeaturedVideos
                .OrderBy(v => v.DisplayOrder)
                .ToListAsync();
            return View(videos);
        }

        // GET: Admin/FeaturedVideos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/FeaturedVideos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FeaturedVideo video)
        {
            if (ModelState.IsValid)
            {
                video.CreatedDate = DateTime.UtcNow;
                _context.FeaturedVideos.Add(video);
                await _context.SaveChangesAsync();
                await SaveFeaturedVideosToJsonAsync();
                TempData["SuccessMessage"] = "Featured video created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(video);
        }

        // GET: Admin/FeaturedVideos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var video = await _context.FeaturedVideos.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }
            return View(video);
        }

        // POST: Admin/FeaturedVideos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FeaturedVideo video)
        {
            if (id != video.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    video.UpdatedDate = DateTime.UtcNow;
                    _context.Update(video);
                    await _context.SaveChangesAsync();
                    await SaveFeaturedVideosToJsonAsync();
                    TempData["SuccessMessage"] = "Featured video updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideoExists(video.Id))
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
            return View(video);
        }

        // GET: Admin/FeaturedVideos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var video = await _context.FeaturedVideos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (video == null)
            {
                return NotFound();
            }

            return View(video);
        }

        // POST: Admin/FeaturedVideos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var video = await _context.FeaturedVideos.FindAsync(id);
            if (video != null)
            {
                video.IsDeleted = true;
                _context.Update(video);
                await _context.SaveChangesAsync();
                await SaveFeaturedVideosToJsonAsync();
                TempData["SuccessMessage"] = "Featured video deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/FeaturedVideos/SyncToJson
        [HttpPost]
        public async Task<IActionResult> SyncToJson()
        {
            try
            {
                await SaveFeaturedVideosToJsonAsync();
                TempData["SuccessMessage"] = "Successfully synced all featured videos to JSON file!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error syncing to JSON: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ImportFromJson()
        {
            try
            {
                var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "JsonData", "FeaturedVideosDatabase.json");
                
                if (!System.IO.File.Exists(jsonPath))
                {
                    TempData["ErrorMessage"] = "JSON file not found!";
                    return RedirectToAction(nameof(Index));
                }

                var jsonContent = await System.IO.File.ReadAllTextAsync(jsonPath);
                var videos = System.Text.Json.JsonSerializer.Deserialize<List<FeaturedVideo>>(jsonContent);

                if (videos == null || !videos.Any())
                {
                    TempData["ErrorMessage"] = "No featured videos found in JSON file!";
                    return RedirectToAction(nameof(Index));
                }

                var validTitles = videos.Select(v => v.Title).ToHashSet();
                var videosToDelete = await _context.FeaturedVideos
                    .Where(v => !validTitles.Contains(v.Title))
                    .ToListAsync();

                _context.FeaturedVideos.RemoveRange(videosToDelete);

                int imported = 0;
                int updated = 0;

                foreach (var video in videos)
                {
                    var existing = await _context.FeaturedVideos
                        .FirstOrDefaultAsync(v => v.Title == video.Title);

                    if (existing == null)
                    {
                        var newVideo = new FeaturedVideo
                        {
                            Title = video.Title,
                            Description = video.Description,
                            YouTubeUrl = video.YouTubeUrl,
                            IsActive = video.IsActive,
                            DisplayOrder = video.DisplayOrder,
                            CreatedDate = DateTime.UtcNow,
                            IsDeleted = false
                        };
                        _context.FeaturedVideos.Add(newVideo);
                        imported++;
                    }
                    else
                    {
                        existing.Description = video.Description;
                        existing.YouTubeUrl = video.YouTubeUrl;
                        existing.IsActive = video.IsActive;
                        existing.DisplayOrder = video.DisplayOrder;
                        existing.UpdatedDate = DateTime.UtcNow;
                        existing.IsDeleted = false;
                        updated++;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Import complete! Imported: {imported}, Updated: {updated}, Removed: {videosToDelete.Count}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error importing from JSON: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task SaveFeaturedVideosToJsonAsync()
        {
            var videos = await _context.FeaturedVideos
                .Where(v => !v.IsDeleted)
                .OrderBy(v => v.DisplayOrder)
                .Select(v => new
                {
                    v.Title,
                    v.Description,
                    v.YouTubeUrl,
                    v.IsActive,
                    v.DisplayOrder,
                    v.CreatedDate
                })
                .ToListAsync();

            var jsonData = System.Text.Json.JsonSerializer.Serialize(videos, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "JsonData", "FeaturedVideosDatabase.json");
            Directory.CreateDirectory(Path.GetDirectoryName(jsonPath)!);
            await System.IO.File.WriteAllTextAsync(jsonPath, jsonData);
        }

        private bool VideoExists(int id)
        {
            return _context.FeaturedVideos.Any(e => e.Id == id);
        }
    }
}
