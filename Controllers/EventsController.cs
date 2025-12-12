using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;
using MarutiTrainingPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarutiTrainingPortal.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHtmlSanitizerService _htmlSanitizer;

        public EventsController(ApplicationDbContext context, IHtmlSanitizerService htmlSanitizer)
        {
            _context = context;
            _htmlSanitizer = htmlSanitizer;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var events = await _context.TrainingEvents.OrderBy(e => e.StartDate).ToListAsync();
            return View(events);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var trainingEvent = await _context.TrainingEvents.FirstOrDefaultAsync(e => e.Id == id);
            if (trainingEvent == null)
                return NotFound();

            return View(trainingEvent);
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
        public async Task<IActionResult> Create(TrainingEvent trainingEvent)
        {
            // Server-side role check
            if (!User.IsInRole("Admin"))
                return Forbid();

            if (ModelState.IsValid)
            {
                // Sanitize HTML input before saving
                trainingEvent.Description = _htmlSanitizer.Sanitize(trainingEvent.Description);

                _context.TrainingEvents.Add(trainingEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trainingEvent);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var trainingEvent = await _context.TrainingEvents.FirstOrDefaultAsync(e => e.Id == id);
            if (trainingEvent == null)
                return NotFound();

            return View(trainingEvent);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainingEvent trainingEvent)
        {
            // Server-side role check
            if (!User.IsInRole("Admin"))
                return Forbid();

            if (id != trainingEvent.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Sanitize HTML input before saving
                trainingEvent.Description = _htmlSanitizer.Sanitize(trainingEvent.Description);

                _context.TrainingEvents.Update(trainingEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trainingEvent);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Server-side role check
            if (!User.IsInRole("Admin"))
                return Forbid();

            var trainingEvent = await _context.TrainingEvents.FirstOrDefaultAsync(e => e.Id == id);
            if (trainingEvent == null)
                return NotFound();

            _context.TrainingEvents.Remove(trainingEvent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
