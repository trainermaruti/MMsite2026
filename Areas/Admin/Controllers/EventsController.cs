using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;
using Ganss.Xss;
using System.Text;

namespace MarutiTrainingPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HtmlSanitizer _sanitizer;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<EventsController> _logger;

        public EventsController(ApplicationDbContext context, HtmlSanitizer sanitizer, IWebHostEnvironment environment, ILogger<EventsController> logger)
        {
            _context = context;
            _sanitizer = sanitizer;
            _environment = environment;
            _logger = logger;
        }

        // GET: Admin/Events
        public async Task<IActionResult> Index(int page = 1, string? q = null, string filter = "All")
        {
            const int pageSize = 10;
            
            var query = _context.TrainingEvents
                .Where(e => !e.IsDeleted)
                .AsQueryable();

            // Apply search
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(e => 
                    e.Title.Contains(q) || 
                    e.Location.Contains(q) ||
                    (e.Summary != null && e.Summary.Contains(q)));
            }

            // Apply filter
            var now = DateTime.UtcNow;
            query = filter switch
            {
                "Upcoming" => query.Where(e => e.StartDate > now && e.Status != "Draft"),
                "Past" => query.Where(e => e.EndDate < now),
                "Draft" => query.Where(e => e.Status == "Draft"),
                "Open" => query.Where(e => e.Status == "Open"),
                "Closed" => query.Where(e => e.Status == "Closed"),
                _ => query
            };

            var totalCount = await query.CountAsync();
            var events = await query
                .OrderByDescending(e => e.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.TotalCount = totalCount;
            ViewBag.SearchQuery = q;
            ViewBag.Filter = filter;

            return View(events);
        }

        // GET: Admin/Events/Create
        public IActionResult Create()
        {
            ViewBag.TimeZones = GetTimeZones();
            return View(new TrainingEvent 
            { 
                StartDate = DateTime.Now.AddDays(7),
                EndDate = DateTime.Now.AddDays(7).AddHours(2),
                TimeZone = "UTC",
                Status = "Draft"
            });
        }

        // POST: Admin/Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainingEvent trainingEvent, IFormFile? bannerFile)
        {
            if (trainingEvent.StartDate >= trainingEvent.EndDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date");
            }

            if (ModelState.IsValid)
            {
                // Sanitize HTML content
                trainingEvent.Description = _sanitizer.Sanitize(trainingEvent.Description);

                // Handle banner upload
                if (bannerFile != null && bannerFile.Length > 0)
                {
                    trainingEvent.BannerUrl = await UploadBannerAsync(bannerFile);
                }

                trainingEvent.CreatedDate = DateTime.UtcNow;
                _context.TrainingEvents.Add(trainingEvent);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TimeZones = GetTimeZones();
            return View(trainingEvent);
        }

        // GET: Admin/Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            
            var trainingEvent = await _context.TrainingEvents.FindAsync(id);
            if (trainingEvent == null || trainingEvent.IsDeleted) return NotFound();

            ViewBag.TimeZones = GetTimeZones();
            return View(trainingEvent);
        }

        // POST: Admin/Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainingEvent trainingEvent, IFormFile? bannerFile)
        {
            if (id != trainingEvent.Id) return NotFound();

            if (trainingEvent.StartDate >= trainingEvent.EndDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Sanitize HTML content
                    trainingEvent.Description = _sanitizer.Sanitize(trainingEvent.Description);

                    // Handle banner upload
                    if (bannerFile != null && bannerFile.Length > 0)
                    {
                        trainingEvent.BannerUrl = await UploadBannerAsync(bannerFile);
                    }

                    trainingEvent.UpdatedDate = DateTime.UtcNow;
                    _context.Update(trainingEvent);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(trainingEvent.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TimeZones = GetTimeZones();
            return View(trainingEvent);
        }

        // POST: Admin/Events/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var trainingEvent = await _context.TrainingEvents.FindAsync(id);
            if (trainingEvent != null)
            {
                trainingEvent.IsDeleted = true;
                trainingEvent.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Events/ViewRegistrants/5
        public async Task<IActionResult> ViewRegistrants(int id)
        {
            var eventItem = await _context.TrainingEvents.FindAsync(id);
            if (eventItem == null) return NotFound();

            var registrations = await _context.TrainingEventRegistrations
                .Where(r => r.TrainingEventId == id && !r.IsDeleted)
                .OrderByDescending(r => r.RegisteredAt)
                .ToListAsync();

            ViewBag.EventTitle = eventItem.Title;
            ViewBag.EventId = id;
            return PartialView("_RegistrantsModal", registrations);
        }

        // GET: Admin/Events/ExportRegistrations/5
        public async Task<IActionResult> ExportRegistrations(int id)
        {
            var eventItem = await _context.TrainingEvents.FindAsync(id);
            if (eventItem == null) return NotFound();

            var registrations = await _context.TrainingEventRegistrations
                .Where(r => r.TrainingEventId == id && !r.IsDeleted)
                .OrderByDescending(r => r.RegisteredAt)
                .ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("Name,Email,Phone,RegisteredAt,EventTitle,Notes");

            foreach (var reg in registrations)
            {
                csv.AppendLine($"\"{reg.Name}\",\"{reg.Email}\",\"{reg.Phone ?? ""}\",\"{reg.RegisteredAt:O}\",\"{eventItem.Title}\",\"{reg.Notes ?? ""}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var fileName = $"registrations_{eventItem.Title.Replace(" ", "_")}_{DateTime.UtcNow:yyyyMMdd}.csv";

            return File(bytes, "text/csv", fileName);
        }

        // POST: Admin/Events/ToggleStatus
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, string status)
        {
            try
            {
                var eventItem = await _context.TrainingEvents.FindAsync(id);
                if (eventItem == null)
                    return Json(new { success = false, message = "Event not found" });

                var validStatuses = new[] { "Draft", "Upcoming", "Open", "Closed" };
                if (!validStatuses.Contains(status))
                    return Json(new { success = false, message = "Invalid status" });

                eventItem.Status = status;
                eventItem.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Status updated successfully", status = status });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling event status");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        private bool EventExists(int id)
        {
            return _context.TrainingEvents.Any(e => e.Id == id && !e.IsDeleted);
        }

        private async Task<string> UploadBannerAsync(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Invalid file type");

            if (file.Length > 5 * 1024 * 1024) // 5MB
                throw new InvalidOperationException("File size exceeds 5MB");

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "events");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/uploads/events/{uniqueFileName}";
        }

        private List<(string Id, string DisplayName)> GetTimeZones()
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones()
                .Select(tz => (
                    Id: tz.Id, 
                    DisplayName: $"(UTC{tz.BaseUtcOffset:hh\\:mm}) {tz.DisplayName}"
                ))
                .OrderBy(tz => tz.DisplayName)
                .ToList();

            return timeZones;
        }
    }
}
