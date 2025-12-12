using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;

namespace MarutiTrainingPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TrainingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Trainings
        public async Task<IActionResult> Index()
        {
            var trainings = await _context.Trainings.OrderByDescending(t => t.DeliveryDate).ToListAsync();
            return View(trainings);
        }

        // GET: Admin/Trainings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Trainings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Training training)
        {
            if (ModelState.IsValid)
            {
                _context.Trainings.Add(training);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Training created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(training);
        }

        // GET: Admin/Trainings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var training = await _context.Trainings.FindAsync(id);
            if (training == null) return NotFound();
            return View(training);
        }

        // POST: Admin/Trainings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Training training)
        {
            if (id != training.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(training);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Training updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingExists(training.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(training);
        }

        // POST: Admin/Trainings/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var training = await _context.Trainings.FindAsync(id);
            if (training != null)
            {
                _context.Trainings.Remove(training);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Training deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TrainingExists(int id)
        {
            return _context.Trainings.Any(e => e.Id == id);
        }
    }
}
