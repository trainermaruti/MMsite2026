using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;
using MarutiTrainingPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarutiTrainingPortal.Controllers
{
    public class TrainingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHtmlSanitizerService _htmlSanitizer;

        public TrainingsController(ApplicationDbContext context, IHtmlSanitizerService htmlSanitizer)
        {
            _context = context;
            _htmlSanitizer = htmlSanitizer;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var trainings = await _context.Trainings.OrderByDescending(t => t.DeliveryDate).ToListAsync();
            return View(trainings);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var training = await _context.Trainings.FirstOrDefaultAsync(t => t.Id == id);
            if (training == null)
                return NotFound();

            return View(training);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Training training)
        {
            // Server-side role check
            if (!User.IsInRole("Admin"))
                return Forbid();

            if (ModelState.IsValid)
            {
                // Sanitize HTML input before saving
                training.Description = _htmlSanitizer.Sanitize(training.Description);
                training.Topics = _htmlSanitizer.Sanitize(training.Topics);

                _context.Trainings.Add(training);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(training);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var training = await _context.Trainings.FirstOrDefaultAsync(t => t.Id == id);
            if (training == null)
                return NotFound();

            return View(training);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Training training)
        {
            // Server-side role check
            if (!User.IsInRole("Admin"))
                return Forbid();

            if (id != training.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Sanitize HTML input before saving
                training.Description = _htmlSanitizer.Sanitize(training.Description);
                training.Topics = _htmlSanitizer.Sanitize(training.Topics);

                _context.Trainings.Update(training);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(training);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Server-side role check
            if (!User.IsInRole("Admin"))
                return Forbid();

            var training = await _context.Trainings.FirstOrDefaultAsync(t => t.Id == id);
            if (training == null)
                return NotFound();

            _context.Trainings.Remove(training);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
