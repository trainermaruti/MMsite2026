using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarutiTrainingPortal.Data;

namespace MarutiTrainingPortal.Controllers
{
    /// <summary>
    /// API controller for training events calendar integration.
    /// Provides endpoints for FullCalendar.js to consume event data.
    /// Public access - no authentication required for viewing calendar.
    /// </summary>
    [Route("api/events")]
    [ApiController]
    public class EventsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EventsApiController> _logger;

        public EventsApiController(ApplicationDbContext context, ILogger<EventsApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Returns events in FullCalendar format for calendar display.
        /// GET /api/events/calendar
        /// Returns events from today onwards (excludes past events).
        /// </summary>
        [HttpGet("calendar")]
        public async Task<IActionResult> GetCalendarEvents()
        {
            try
            {
                var now = DateTime.UtcNow;

                var events = await _context.TrainingEvents
                    .Where(e => !e.IsDeleted && e.StartDate >= now.AddDays(-30)) // Include events from last 30 days
                    .OrderBy(e => e.StartDate)
                    .Select(e => new
                    {
                        id = e.Id,
                        title = e.Title,
                        start = e.StartDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                        end = e.EndDate.HasValue ? e.EndDate.Value.ToString("yyyy-MM-ddTHH:mm:ss") : e.StartDate.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ss"),
                        url = $"/Events/Details/{e.Id}",
                        description = e.Description,
                        location = e.Location,
                        eventType = e.EventType,
                        backgroundColor = GetEventColor(e.EventType),
                        borderColor = GetEventColor(e.EventType),
                        textColor = "#ffffff",
                        extendedProps = new
                        {
                            registeredParticipants = e.RegisteredParticipants,
                            maxParticipants = e.MaxParticipants,
                            isFull = e.RegisteredParticipants >= e.MaxParticipants,
                            registrationLink = e.RegistrationLink
                        }
                    })
                    .ToListAsync();

                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching calendar events");
                return StatusCode(500, new { error = "Failed to fetch calendar events" });
            }
        }

        /// <summary>
        /// Returns event details by ID for modal display.
        /// GET /api/events/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventDetails(int id)
        {
            try
            {
                var eventDetails = await _context.TrainingEvents
                    .Where(e => e.Id == id && !e.IsDeleted)
                    .Select(e => new
                    {
                        id = e.Id,
                        title = e.Title,
                        description = e.Description,
                        startDate = e.StartDate,
                        endDate = e.EndDate,
                        location = e.Location,
                        eventType = e.EventType,
                        maxParticipants = e.MaxParticipants,
                        registeredParticipants = e.RegisteredParticipants,
                        isFull = e.RegisteredParticipants >= e.MaxParticipants,
                        registrationLink = e.RegistrationLink
                    })
                    .FirstOrDefaultAsync();

                if (eventDetails == null)
                {
                    return NotFound(new { error = "Event not found" });
                }

                return Ok(eventDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching event details for ID {EventId}", id);
                return StatusCode(500, new { error = "Failed to fetch event details" });
            }
        }

        /// <summary>
        /// Helper method to assign colors based on event type.
        /// </summary>
        private static string GetEventColor(string eventType)
        {
            return eventType?.ToLower() switch
            {
                "webinar" => "#6366f1",      // Indigo
                "workshop" => "#8b5cf6",     // Purple
                "bootcamp" => "#ec4899",     // Pink
                "conference" => "#10b981",   // Green
                "training" => "#f59e0b",     // Amber
                "certification" => "#ef4444", // Red
                _ => "#6366f1"               // Default indigo
            };
        }
    }
}
