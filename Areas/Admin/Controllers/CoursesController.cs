using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;
using MarutiTrainingPortal.Services;

namespace MarutiTrainingPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CourseImportService _courseImportService;

        public CoursesController(ApplicationDbContext context, CourseImportService courseImportService)
        {
            _context = context;
            _courseImportService = courseImportService;
        }

        // GET: Admin/Courses
        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses.OrderByDescending(c => c.CreatedDate).ToListAsync();
            return View(courses);
        }

        // GET: Admin/Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            if (ModelState.IsValid)
            {
                course.CreatedDate = DateTime.Now;
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // GET: Admin/Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();
            return View(course);
        }

        // POST: Admin/Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course)
        {
            if (id != course.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Course updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // POST: Admin/Courses/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Courses/ImportPreview
        public IActionResult ImportPreview()
        {
            var skillTechCourses = _courseImportService.GetAvailableSkillTechCourses();
            return View(skillTechCourses);
        }

        // GET: Admin/Courses/ImportCourse - Get course data as JSON for preview
        [HttpGet]
        public IActionResult GetCourseData(string title)
        {
            var courses = _courseImportService.GetAvailableSkillTechCourses();
            var course = courses.FirstOrDefault(c => c["Title"] == title);
            
            if (course == null)
                return NotFound();

            return Json(course);
        }

        // POST: Admin/Courses/ImportConfirm - Import the selected course
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportConfirm([FromBody] Course course)
        {
            if (ModelState.IsValid)
            {
                // Check if course already exists
                var existingCourse = await _context.Courses
                    .FirstOrDefaultAsync(c => c.Title == course.Title);

                if (existingCourse != null)
                {
                    return Json(new { success = false, message = "Course already exists in the database." });
                }

                course.CreatedDate = DateTime.Now;
                course.PublishedDate = DateTime.Now;
                course.TotalEnrollments = 0;
                course.Rating = 0;
                
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                
                return Json(new { success = true, message = "Course imported successfully!" });
            }

            return Json(new { success = false, message = "Invalid course data." });
        }

        [HttpPost]
        public async Task<IActionResult> ImportFromJson()
        {
            try
            {
                var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "JsonData", "CoursesDatabase.json");
                
                if (!System.IO.File.Exists(jsonPath))
                {
                    TempData["ErrorMessage"] = "JSON file not found!";
                    return RedirectToAction(nameof(Index));
                }

                var jsonContent = await System.IO.File.ReadAllTextAsync(jsonPath);
                var courses = System.Text.Json.JsonSerializer.Deserialize<List<Course>>(jsonContent);

                if (courses == null || !courses.Any())
                {
                    TempData["ErrorMessage"] = "No courses found in JSON file!";
                    return RedirectToAction(nameof(Index));
                }

                var validTitles = courses.Select(c => c.Title).ToHashSet();
                var coursesToDelete = await _context.Courses
                    .Where(c => !validTitles.Contains(c.Title))
                    .ToListAsync();

                _context.Courses.RemoveRange(coursesToDelete);

                int imported = 0;
                int updated = 0;

                foreach (var course in courses)
                {
                    var existing = await _context.Courses
                        .FirstOrDefaultAsync(c => c.Title == course.Title);

                    if (existing == null)
                    {
                        var newCourse = new Course
                        {
                            Title = course.Title,
                            Description = course.Description,
                            Category = course.Category,
                            Level = course.Level,
                            DurationMinutes = course.DurationMinutes,
                            VideoUrl = course.VideoUrl,
                            ThumbnailUrl = course.ThumbnailUrl,
                            Price = course.Price,
                            SkillTechUrl = course.SkillTechUrl,
                            CreatedDate = DateTime.UtcNow,
                            IsDeleted = false
                        };
                        _context.Courses.Add(newCourse);
                        imported++;
                    }
                    else
                    {
                        existing.Description = course.Description;
                        existing.Category = course.Category;
                        existing.Level = course.Level;
                        existing.DurationMinutes = course.DurationMinutes;
                        existing.VideoUrl = course.VideoUrl;
                        existing.ThumbnailUrl = course.ThumbnailUrl;
                        existing.Price = course.Price;
                        existing.SkillTechUrl = course.SkillTechUrl;
                        existing.UpdatedDate = DateTime.UtcNow;
                        existing.IsDeleted = false;
                        updated++;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Import complete! Imported: {imported}, Updated: {updated}, Removed: {coursesToDelete.Count}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error importing from JSON: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SyncToJson()
        {
            try
            {
                await SaveCoursesToJsonAsync();
                TempData["SuccessMessage"] = "Successfully synced all courses to JSON file!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error syncing to JSON: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task SaveCoursesToJsonAsync()
        {
            var courses = await _context.Courses
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Title)
                .Select(c => new
                {
                    c.Title,
                    c.Description,
                    c.Category,
                    c.Level,
                    c.DurationMinutes,
                    c.VideoUrl,
                    c.ThumbnailUrl,
                    c.Price,
                    c.SkillTechUrl
                })
                .ToListAsync();

            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "JsonData", "CoursesDatabase.json");
            Directory.CreateDirectory(Path.GetDirectoryName(jsonPath));

            var options = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            };

            var jsonString = System.Text.Json.JsonSerializer.Serialize(courses, options);
            await System.IO.File.WriteAllTextAsync(jsonPath, jsonString);
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
