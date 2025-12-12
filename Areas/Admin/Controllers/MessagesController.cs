using MarutiTrainingPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarutiTrainingPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MessagesController : Controller
    {
        private readonly IContactMessageService _messageService;
        private readonly IMessageCleanupService _cleanupService;

        public MessagesController(
            IContactMessageService messageService,
            IMessageCleanupService cleanupService)
        {
            _messageService = messageService;
            _cleanupService = cleanupService;
        }

        // GET: Admin/Messages
        public async Task<IActionResult> Index(int page = 1, string? q = null, string? filter = "All")
        {
            const int pageSize = 20;

            var (messages, totalCount, newCount, totalMessages) = await _messageService.GetInboxAsync(
                page, 
                pageSize, 
                q, 
                filter);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.SearchQuery = q;
            ViewBag.Filter = filter ?? "All";
            ViewBag.NewCount = newCount;
            ViewBag.TotalMessages = totalMessages;

            return View(messages);
        }

        // GET: Admin/Messages/GetMessage/5
        [HttpGet]
        public async Task<IActionResult> GetMessage(int id)
        {
            var message = await _messageService.GetMessageAsync(id);
            
            if (message == null)
            {
                return NotFound(new { success = false, error = "Message not found" });
            }

            // Mark as read when viewing
            await _messageService.MarkAsReadAsync(id, true);

            return Json(new
            {
                success = true,
                id = message.Id,
                name = message.Name,
                email = message.Email,
                phoneNumber = message.PhoneNumber ?? "",
                subject = message.Subject,
                message = message.Message,
                createdDate = message.CreatedDate.ToString("MMM dd, yyyy 'at' h:mm tt"),
                isRead = true, // Now marked as read
                status = message.Status,
                eventId = message.EventId,
                eventTitle = message.Event?.Title ?? ""
            });
        }

        // POST: Admin/Messages/MarkAsRead/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id, bool isRead = true)
        {
            var success = await _messageService.MarkAsReadAsync(id, isRead);

            if (!success)
            {
                return NotFound(new { success = false, error = "Message not found" });
            }

            return Json(new { success = true, isRead });
        }

        // POST: Admin/Messages/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _messageService.DeleteMessageAsync(id);

            if (!success)
            {
                return NotFound(new { success = false, error = "Message not found" });
            }

            TempData["SuccessMessage"] = "Message deleted successfully.";
            return Json(new { success = true });
        }

        // GET: Admin/Messages/Export?filter=All
        [HttpGet]
        public async Task<IActionResult> Export(string? filter = "All")
        {
            var csvBytes = await _messageService.ExportToCsvAsync(filter);

            var fileName = $"contact-messages-{DateTime.UtcNow:yyyyMMdd-HHmmss}.csv";

            return File(csvBytes, "text/csv", fileName);
        }

        // GET: Admin/Messages/Cleanup - Show cleanup information
        [HttpGet]
        public async Task<IActionResult> Cleanup()
        {
            var expiredCount = await _cleanupService.GetExpiredMessageCountAsync(daysOld: 28);
            var expiringMessages = await _cleanupService.GetExpiringMessagesAsync(daysOld: 21);

            var viewModel = new Dictionary<string, object>
            {
                { "ExpiredCount", expiredCount },
                { "ExpiringCount", expiringMessages.Count },
                { "ExpiringMessages", expiringMessages.OrderBy(m => m.CreatedDate).ToList() }
            };

            return View(viewModel);
        }

        // POST: Admin/Messages/DeleteExpired - Manually trigger 28-day cleanup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExpired()
        {
            try
            {
                var deletedCount = await _cleanupService.DeleteExpiredMessagesAsync(daysOld: 28);
                TempData["SuccessMessage"] = $"Successfully deleted {deletedCount} message(s) older than 28 days.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error during cleanup: {ex.Message}";
            }

            return RedirectToAction(nameof(Cleanup));
        }

        // POST: Admin/Messages/DeleteById - Delete specific message manually
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteById(int id)
        {
            var success = await _cleanupService.DeleteMessageAsync(id);

            if (!success)
            {
                return BadRequest(new { success = false, error = "Message not found" });
            }

            TempData["SuccessMessage"] = "Message deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
