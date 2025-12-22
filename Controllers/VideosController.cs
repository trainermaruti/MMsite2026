using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarutiTrainingPortal.Controllers
{
    public class VideosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VideosController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays the complete Video Library grid.
        /// Sorting: Newest first (Id DESC) to ensure "Recently Added" order.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var videos = await _context.Videos
                .Where(v => v.IsActive)
                .OrderByDescending(v => v.Id)
                .ToListAsync();

            return View(videos);
        }
    }
}
