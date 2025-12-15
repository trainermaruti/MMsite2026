using MarutiTrainingPortal.Data;
using Microsoft.EntityFrameworkCore;

namespace MarutiTrainingPortal.Services;

/// <summary>
/// Background service to automatically clean up contact messages older than 28 days
/// </summary>
public class ContactMessageCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ContactMessageCleanupService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); // Run once per day

    public ContactMessageCleanupService(
        IServiceProvider serviceProvider,
        ILogger<ContactMessageCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Contact Message Cleanup Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupOldMessagesAsync(stoppingToken);
                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when application is shutting down
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during contact message cleanup");
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken); // Wait 30 min before retry
            }
        }

        _logger.LogInformation("Contact Message Cleanup Service stopped");
    }

    private async Task CleanupOldMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Get messages older than 28 days
        var cutoffDate = DateTime.Now.AddDays(-28);
        
        var oldMessages = await context.ContactMessages
            .Where(m => !m.IsDeleted && m.CreatedDate < cutoffDate)
            .ToListAsync(cancellationToken);

        if (oldMessages.Any())
        {
            foreach (var message in oldMessages)
            {
                message.IsDeleted = true;
                message.UpdatedDate = DateTime.Now;
            }

            await context.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation(
                "Auto-cleanup completed: Soft-deleted {Count} contact messages older than 28 days (before {CutoffDate})",
                oldMessages.Count,
                cutoffDate.ToString("yyyy-MM-dd"));
        }
        else
        {
            _logger.LogInformation("Contact message cleanup: No messages older than 28 days found");
        }
    }
}
