using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;
using System.Text;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;

namespace MarutiTrainingPortal.Areas.Admin.Controllers
{
    /// <summary>
    /// Admin controller for managing leads (contact messages).
    /// Provides status management, filtering, and export functionality (CSV/XLSX).
    /// Requires Admin role authentication.
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LeadsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LeadsController> _logger;

        public LeadsController(ApplicationDbContext context, ILogger<LeadsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Displays the leads management dashboard.
        /// GET /Admin/Leads
        /// </summary>
        public async Task<IActionResult> Index(string? status = null, string? search = null, int page = 1, int pageSize = 20)
        {
            var query = _context.ContactMessages
                .Include(c => c.Event)
                .Where(c => !c.IsDeleted)
                .AsQueryable();

            // Filter by status
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(c => c.Status == status);
            }

            // Search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c =>
                    c.Name.Contains(search) ||
                    c.Email.Contains(search) ||
                    (c.Message != null && c.Message.Contains(search))
                );
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var leads = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Pass filter values to view
            ViewBag.CurrentStatus = status ?? "All";
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Get status counts for filter badges
            ViewBag.NewCount = await _context.ContactMessages.Where(c => !c.IsDeleted && c.Status == "New").CountAsync();
            ViewBag.ContactedCount = await _context.ContactMessages.Where(c => !c.IsDeleted && c.Status == "Contacted").CountAsync();
            ViewBag.QualifiedCount = await _context.ContactMessages.Where(c => !c.IsDeleted && c.Status == "Qualified").CountAsync();
            ViewBag.ConvertedCount = await _context.ContactMessages.Where(c => !c.IsDeleted && c.Status == "Converted").CountAsync();
            ViewBag.ClosedCount = await _context.ContactMessages.Where(c => !c.IsDeleted && c.Status == "Closed").CountAsync();

            return View(leads);
        }

        /// <summary>
        /// Updates the status of a lead and creates an audit log entry.
        /// POST /Admin/Leads/UpdateStatus
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status, string? notes = null)
        {
            try
            {
                var lead = await _context.ContactMessages.FindAsync(id);
                if (lead == null || lead.IsDeleted)
                {
                    return Json(new { success = false, message = "Lead not found" });
                }

                var oldStatus = lead.Status;
                lead.Status = status;
                lead.UpdatedDate = DateTime.UtcNow;

                // Create audit log entry
                var auditLog = new LeadAuditLog
                {
                    ContactMessageId = id,
                    Action = "Status Changed",
                    OldValue = oldStatus,
                    NewValue = status,
                    ChangedBy = User.Identity?.Name ?? "Admin",
                    Notes = notes,
                    CreatedDate = DateTime.UtcNow
                };

                _context.LeadAuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Lead {LeadId} status changed from {OldStatus} to {NewStatus} by {User}",
                    id, oldStatus, status, User.Identity?.Name);

                return Json(new { success = true, message = "Status updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lead status for ID {LeadId}", id);
                return Json(new { success = false, message = "Failed to update status" });
            }
        }

        /// <summary>
        /// Exports leads to CSV format.
        /// GET /Admin/Leads/ExportCsv
        /// FREE: Uses CsvHelper library (MIT license)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExportCsv(string? status = null)
        {
            try
            {
                var query = _context.ContactMessages
                    .Include(c => c.Event)
                    .Where(c => !c.IsDeleted)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(status) && status != "All")
                {
                    query = query.Where(c => c.Status == status);
                }

                var leads = await query
                    .OrderByDescending(c => c.CreatedDate)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Email,
                        c.Phone,
                        c.Message,
                        c.Status,
                        EventName = c.Event != null ? c.Event.Title : "N/A",
                        CreatedDate = c.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        UpdatedDate = c.UpdatedDate.HasValue ? c.UpdatedDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""
                    })
                    .ToListAsync();

                using var memoryStream = new MemoryStream();
                using var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
                using var csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture));

                await csvWriter.WriteRecordsAsync(leads);
                await streamWriter.FlushAsync();

                var fileName = $"leads_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
                return File(memoryStream.ToArray(), "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting leads to CSV");
                TempData["ErrorMessage"] = "Failed to export leads to CSV";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Exports leads to Excel (XLSX) format.
        /// GET /Admin/Leads/ExportExcel
        /// FREE: Uses EPPlus Community Edition (limited to non-commercial use without license)
        /// For commercial use, purchase EPPlus license or use CSV export instead.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExportExcel(string? status = null)
        {
            try
            {
                // Set EPPlus license context (required for EPPlus 5.0+)
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                var query = _context.ContactMessages
                    .Include(c => c.Event)
                    .Where(c => !c.IsDeleted)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(status) && status != "All")
                {
                    query = query.Where(c => c.Status == status);
                }

                var leads = await query
                    .OrderByDescending(c => c.CreatedDate)
                    .ToListAsync();

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Leads");

                // Headers
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Email";
                worksheet.Cells[1, 4].Value = "Phone";
                worksheet.Cells[1, 5].Value = "Message";
                worksheet.Cells[1, 6].Value = "Status";
                worksheet.Cells[1, 7].Value = "Event";
                worksheet.Cells[1, 8].Value = "Created Date";
                worksheet.Cells[1, 9].Value = "Updated Date";

                // Style headers
                using (var range = worksheet.Cells[1, 1, 1, 9])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(99, 102, 241));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                // Data
                for (int i = 0; i < leads.Count; i++)
                {
                    var lead = leads[i];
                    var row = i + 2;

                    worksheet.Cells[row, 1].Value = lead.Id;
                    worksheet.Cells[row, 2].Value = lead.Name;
                    worksheet.Cells[row, 3].Value = lead.Email;
                    worksheet.Cells[row, 4].Value = lead.Phone;
                    worksheet.Cells[row, 5].Value = lead.Message;
                    worksheet.Cells[row, 6].Value = lead.Status;
                    worksheet.Cells[row, 7].Value = lead.Event?.Title ?? "N/A";
                    worksheet.Cells[row, 8].Value = lead.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss");
                    worksheet.Cells[row, 9].Value = lead.UpdatedDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var fileName = $"leads_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting leads to Excel");
                TempData["ErrorMessage"] = "Failed to export leads to Excel. Using CSV export instead.";
                return RedirectToAction(nameof(ExportCsv), new { status });
            }
        }

        /// <summary>
        /// Displays audit log for a specific lead.
        /// GET /Admin/Leads/AuditLog/{id}
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> AuditLog(int id)
        {
            var lead = await _context.ContactMessages
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (lead == null || lead.IsDeleted)
            {
                return NotFound();
            }

            var auditLogs = await _context.LeadAuditLogs
                .Where(a => a.ContactMessageId == id)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();

            ViewBag.Lead = lead;
            return View(auditLogs);
        }

        /// <summary>
        /// Soft deletes a lead.
        /// POST /Admin/Leads/Delete/{id}
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var lead = await _context.ContactMessages.FindAsync(id);
                if (lead == null)
                {
                    return Json(new { success = false, message = "Lead not found" });
                }

                lead.IsDeleted = true;
                lead.UpdatedDate = DateTime.UtcNow;

                // Create audit log
                var auditLog = new LeadAuditLog
                {
                    ContactMessageId = id,
                    Action = "Deleted",
                    OldValue = lead.Status,
                    NewValue = "Deleted",
                    ChangedBy = User.Identity?.Name ?? "Admin",
                    Notes = "Lead soft deleted",
                    CreatedDate = DateTime.UtcNow
                };

                _context.LeadAuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Lead deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lead {LeadId}", id);
                return Json(new { success = false, message = "Failed to delete lead" });
            }
        }
    }
}
