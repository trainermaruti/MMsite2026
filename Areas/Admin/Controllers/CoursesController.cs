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

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
