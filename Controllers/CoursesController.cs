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
            return View(courses);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course == null)
                return NotFound();

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
    }
}
