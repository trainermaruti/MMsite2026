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
                TempData["SuccessMessage"] = "Featured video deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VideoExists(int id)
        {
            return _context.FeaturedVideos.Any(e => e.Id == id);
        }
    }
}
