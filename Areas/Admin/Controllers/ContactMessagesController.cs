using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarutiTrainingPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ContactMessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ContactMessagesController> _logger;

        public ContactMessagesController(ApplicationDbContext context, ILogger<ContactMessagesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Admin/ContactMessages
        public async Task<IActionResult> Index(int page = 1, string status = "All", bool unreadOnly = false)
        {
            const int pageSize = 20;

            // Auto-cleanup: Soft delete messages older than 28 days
            var cutoffDate = DateTime.Now.AddDays(-28);
            var oldMessages = await _context.ContactMessages
                .Where(m => !m.IsDeleted && m.CreatedDate < cutoffDate)
                .ToListAsync();
            
            if (oldMessages.Any())
            {
                foreach (var msg in oldMessages)
                {
                    msg.IsDeleted = true;
                }
                await _context.SaveChangesAsync();
                _logger.LogInformation("Auto-deleted {Count} messages older than 28 days", oldMessages.Count);
            }

            var query = _context.ContactMessages
                .Where(m => !m.IsDeleted)
                .OrderByDescending(m => m.CreatedDate)
                .AsQueryable();

            // Filter by status
            if (status != "All")
            {
                query = query.Where(m => m.Status == status);
            }

            // Filter unread only
            if (unreadOnly)
            {
                query = query.Where(m => !m.IsRead);
            }

            var totalMessages = await query.CountAsync();
            var messages = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalMessages / (double)pageSize);
            ViewBag.TotalMessages = totalMessages;
            ViewBag.CurrentStatus = status;
            ViewBag.UnreadOnly = unreadOnly;
            ViewBag.UnreadCount = await _context.ContactMessages.CountAsync(m => !m.IsRead && !m.IsDeleted);

            return View(messages);
        }

        // GET: Admin/ContactMessages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.ContactMessages
                .Include(m => m.Event)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

            if (message == null)
            {
                return NotFound();
            }

            // Mark as read
            if (!message.IsRead)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Contact message {Id} marked as read", id);
            }

            return View(message);
        }

        // POST: Admin/ContactMessages/MarkAsRead/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message != null && !message.IsDeleted)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Contact message {Id} marked as read", id);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/ContactMessages/MarkAsUnread/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsUnread(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message != null && !message.IsDeleted)
            {
                message.IsRead = false;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Contact message {Id} marked as unread", id);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/ContactMessages/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message != null && !message.IsDeleted)
            {
                message.Status = status;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Contact message {Id} status updated to {Status}", id, status);
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Admin/ContactMessages/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message != null)
            {
                message.IsDeleted = true;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Contact message {Id} marked as deleted", id);
                TempData["SuccessMessage"] = "Message deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
