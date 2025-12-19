using Microsoft.AspNetCore.Mvc;
using MarutiTrainingPortal.Data;
using Microsoft.EntityFrameworkCore;

namespace MarutiTrainingPortal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(ApplicationDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var status = new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
            DatabaseConnected = false,
            Message = "Application is running"
        };

        try
        {
            // Test database connection
            await _context.Database.CanConnectAsync();
            status = status with { DatabaseConnected = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection failed");
            status = status with 
            { 
                DatabaseConnected = false,
                Message = $"Application running but database connection failed: {ex.Message}"
            };
        }

        return Ok(status);
    }
}
