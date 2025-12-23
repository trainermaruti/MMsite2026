using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models.ViewModels;

namespace MarutiTrainingPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalTrainings = await _context.Trainings.CountAsync(),
                TotalCourses = await _context.Courses.CountAsync(),
                TotalEvents = await _context.TrainingEvents.CountAsync(),
                TotalVideos = await _context.Videos.CountAsync(),
                UnreadContactMessages = await _context.ContactMessages.CountAsync(),
                RecentMessages = await _context.ContactMessages
                    .OrderByDescending(m => m.CreatedDate)
                    .Take(5)
                    .ToListAsync(),
                UpcomingEventsList = await _context.TrainingEvents
                    .Where(e => e.StartDate >= DateTime.Now)
                    .OrderBy(e => e.StartDate)
                    .Take(5)
                    .ToListAsync()
            };

            return View(model);
        }

        /// <summary>
        /// AJAX endpoint for Chart.js data (monthly trainings, courses by category)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetChartData()
        {
            // Double-check authorization at server level
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            // Monthly trainings for last 6 months
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var monthlyTrainings = await _context.Trainings
                .Where(t => t.DeliveryDate.HasValue && t.DeliveryDate.Value >= sixMonthsAgo)
                .GroupBy(t => new { t.DeliveryDate.Value.Year, t.DeliveryDate.Value.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            // Courses by category
            var coursesByCategory = await _context.Courses
                .GroupBy(c => c.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToListAsync();

            return Json(new
            {
                monthlyTrainings = new
                {
                    labels = monthlyTrainings.Select(m => $"{m.Year}-{m.Month:D2}").ToArray(),
                    data = monthlyTrainings.Select(m => m.Count).ToArray()
                },
                coursesByCategory = new
                {
                    labels = coursesByCategory.Select(c => c.Category).ToArray(),
                    data = coursesByCategory.Select(c => c.Count).ToArray()
                }
            });
        }
    }
}
