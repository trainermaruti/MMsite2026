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

        public MessagesController(IContactMessageService messageService)
        {
            _messageService = messageService;
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
    }
}
