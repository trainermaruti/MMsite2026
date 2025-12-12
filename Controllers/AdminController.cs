using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarutiTrainingPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalTrainings = await _context.Trainings.CountAsync(),
                TotalCourses = await _context.Courses.CountAsync(),
                TotalEvents = await _context.TrainingEvents.CountAsync(),
                TotalContactMessages = await _context.ContactMessages.CountAsync(),
                UnreadContactMessages = await _context.ContactMessages.CountAsync(cm => !cm.IsRead),
                TotalStudents = await _context.Profiles
                    .Select(p => p.TotalStudents)
                    .FirstOrDefaultAsync(),
                UpcomingEvents = await _context.TrainingEvents
                    .CountAsync(e => e.StartDate >= DateTime.Now),
                RecentMessages = await _context.ContactMessages
                    .OrderByDescending(cm => cm.CreatedDate)
                    .Take(5)
                    .ToListAsync(),
                UpcomingEventsList = await _context.TrainingEvents
                    .Where(e => e.StartDate >= DateTime.Now)
                    .OrderBy(e => e.StartDate)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var stats = new
            {
                trainings = await _context.Trainings.CountAsync(),
                courses = await _context.Courses.CountAsync(),
                events = await _context.TrainingEvents.CountAsync(),
                messages = await _context.ContactMessages.CountAsync(),
                unreadMessages = await _context.ContactMessages.CountAsync(cm => !cm.IsRead)
            };

            return Json(stats);
        }

        [HttpGet]
        public async Task<IActionResult> GetChartData()
        {
            var monthlyTrainings = await _context.Trainings
                .Where(t => t.DeliveryDate.HasValue && t.DeliveryDate.Value >= DateTime.Now.AddMonths(-6))
                .GroupBy(t => new { t.DeliveryDate.Value.Year, t.DeliveryDate.Value.Month })
                .Select(g => new
                {
                    month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    count = g.Count()
                })
                .OrderBy(x => x.month)
                .ToListAsync();

            var coursesByCategory = await _context.Courses
                .GroupBy(c => c.Category)
                .Select(g => new
                {
                    category = g.Key,
                    count = g.Count()
                })
                .ToListAsync();

            return Json(new { monthlyTrainings, coursesByCategory });
        }
    }
}
