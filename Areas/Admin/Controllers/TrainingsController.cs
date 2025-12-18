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
                await SaveTrainingsToJsonAsync();
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
                    await SaveTrainingsToJsonAsync();
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
                await SaveTrainingsToJsonAsync();
                TempData["SuccessMessage"] = "Training deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TrainingExists(int id)
        {
            return _context.Trainings.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> SyncToJson()
        {
            try
            {
                await SaveTrainingsToJsonAsync();
                TempData["SuccessMessage"] = "Successfully synced all trainings to JSON file!";
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
                var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "JsonData", "TrainingsDatabase.json");
                
                if (!System.IO.File.Exists(jsonPath))
                {
                    TempData["ErrorMessage"] = "JSON file not found!";
                    return RedirectToAction(nameof(Index));
                }

                var jsonContent = await System.IO.File.ReadAllTextAsync(jsonPath);
                var trainings = System.Text.Json.JsonSerializer.Deserialize<List<Training>>(jsonContent);

                if (trainings == null || !trainings.Any())
                {
                    TempData["ErrorMessage"] = "No trainings found in JSON file!";
                    return RedirectToAction(nameof(Index));
                }

                var validTitles = trainings.Select(t => t.Title).ToHashSet();
                var trainingsToDelete = await _context.Trainings
                    .Where(t => !validTitles.Contains(t.Title))
                    .ToListAsync();

                _context.Trainings.RemoveRange(trainingsToDelete);

                int imported = 0;
                int updated = 0;

                foreach (var training in trainings)
                {
                    var existing = await _context.Trainings
                        .FirstOrDefaultAsync(t => t.Title == training.Title);

                    if (existing == null)
                    {
                        var newTraining = new Training
                        {
                            Title = training.Title,
                            Description = training.Description,
                            Company = training.Company,
                            DeliveryDate = training.DeliveryDate,
                            Duration = training.Duration,
                            Topics = training.Topics,
                            ImageUrl = training.ImageUrl,
                            SkillTechUrl = training.SkillTechUrl,
                            CreatedDate = DateTime.UtcNow,
                            IsDeleted = false
                        };
                        _context.Trainings.Add(newTraining);
                        imported++;
                    }
                    else
                    {
                        existing.Description = training.Description;
                        existing.Company = training.Company;
                        existing.DeliveryDate = training.DeliveryDate;
                        existing.Duration = training.Duration;
                        existing.Topics = training.Topics;
                        existing.ImageUrl = training.ImageUrl;
                        existing.SkillTechUrl = training.SkillTechUrl;
                        existing.UpdatedDate = DateTime.UtcNow;
                        existing.IsDeleted = false;
                        updated++;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Import complete! Imported: {imported}, Updated: {updated}, Removed: {trainingsToDelete.Count}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error importing from JSON: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task SaveTrainingsToJsonAsync()
        {
            var trainings = await _context.Trainings
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.Id)
                .Select(t => new
                {
                    t.Title,
                    t.Description,
                    t.Company,
                    t.DeliveryDate,
                    t.Duration,
                    t.Topics,
                    t.ImageUrl,
                    t.SkillTechUrl,
                    t.CreatedDate
                })
                .ToListAsync();

            var jsonData = System.Text.Json.JsonSerializer.Serialize(trainings, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "JsonData", "TrainingsDatabase.json");
            Directory.CreateDirectory(Path.GetDirectoryName(jsonPath)!);
            await System.IO.File.WriteAllTextAsync(jsonPath, jsonData);
        }
    }
}
