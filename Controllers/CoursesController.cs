using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;
using MarutiTrainingPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarutiTrainingPortal.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CourseImportService _courseImportService;
        private readonly IHtmlSanitizerService _htmlSanitizer;

        public CoursesController(ApplicationDbContext context, CourseImportService courseImportService, IHtmlSanitizerService htmlSanitizer)
        {
            _context = context;
            _courseImportService = courseImportService;
            _htmlSanitizer = htmlSanitizer;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses.OrderByDescending(c => c.PublishedDate).ToListAsync();
            
            // Debug: Log thumbnail URLs
            foreach (var course in courses.Take(3))
            {
                Console.WriteLine($"Course: {course.Title}");
                Console.WriteLine($"ThumbnailUrl: {course.ThumbnailUrl}");
            }
            
            return View(courses);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course == null)
                return NotFound();

            // If course has SkillTech URL, redirect to it
            if (!string.IsNullOrEmpty(course.SkillTechUrl))
            {
                return Redirect(course.SkillTechUrl);
            }

            return View(course);
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
        public async Task<IActionResult> Create(Course course)
        {
            // Server-side role check
            if (!User.IsInRole("Admin"))
                return Forbid();

            if (ModelState.IsValid)
            {
                // Sanitize HTML input before saving
                course.Description = _htmlSanitizer.Sanitize(course.Description);

                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                
                // Save to JSON file
                await SaveCoursesToJsonAsync();
                
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course)
        {
            // Server-side role check
            if (!User.IsInRole("Admin"))
                return Forbid();

            if (id != course.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Sanitize HTML input before saving
                course.Description = _htmlSanitizer.Sanitize(course.Description);

                _context.Courses.Update(course);
                await _context.SaveChangesAsync();
                
                // Save to JSON file
                await SaveCoursesToJsonAsync();
                
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // Server-side role check
            if (!User.IsInRole("Admin"))
                return Forbid();

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course == null)
                return NotFound();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            
            // Save to JSON file
            await SaveCoursesToJsonAsync();
            
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportSkillTechCourses()
        {
            // Server-side role check
            if (!User.IsInRole("Admin"))
                return Forbid();

            try
            {
                var importedCourses = await _courseImportService.ImportSkillTechCoursesAsync();
                TempData["SuccessMessage"] = $"Successfully imported {importedCourses.Count} courses from SkillTech Club!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error importing courses: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult ImportOptions()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SyncToJson()
        {
            // Server-side role check
            if (!User.IsInRole("Admin"))
                return Forbid();

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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportFromJson()
        {
            if (!User.IsInRole("Admin"))
                return Forbid();

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

        private async Task SaveCoursesToJsonAsync()
        {
            try
            {
                var courses = await _context.Courses.OrderBy(c => c.Id).ToListAsync();
                var jsonData = courses.Select(c => new
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
                }).ToList();

                var json = System.Text.Json.JsonSerializer.Serialize(jsonData, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });

                var filePath = Path.Combine("JsonData", "CoursesDatabase.json");
                await System.IO.File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving courses to JSON: {ex.Message}");
            }
        }
    }
}
