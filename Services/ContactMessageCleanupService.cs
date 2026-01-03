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
        _logger.LogInformation("Contact Message Cleanup Service started. First cleanup will run in 24 hours.");

        // Wait 24 hours before first cleanup to avoid deleting messages on server restart
        await Task.Delay(_checkInterval, stoppingToken);

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
        var cutoffDate = DateTime.UtcNow.AddDays(-28);
        
        _logger.LogInformation(
            "Running cleanup check. Cutoff date: {CutoffDate}. Messages created before this will be deleted.",
            cutoffDate.ToString("yyyy-MM-dd HH:mm:ss"));
        
        var oldMessages = await context.ContactMessages
            .Where(m => !m.IsDeleted && m.CreatedDate < cutoffDate)
            .ToListAsync(cancellationToken);

        if (oldMessages.Any())
        {
            _logger.LogWarning(
                "Found {Count} messages to delete. Oldest: {OldestDate}, Newest: {NewestDate}",
                oldMessages.Count,
                oldMessages.Min(m => m.CreatedDate).ToString("yyyy-MM-dd HH:mm:ss"),
                oldMessages.Max(m => m.CreatedDate).ToString("yyyy-MM-dd HH:mm:ss"));

            foreach (var message in oldMessages)
            {
                message.IsDeleted = true;
                message.UpdatedDate = DateTime.UtcNow;
            }

            await context.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation(
                "Auto-cleanup completed: Soft-deleted {Count} contact messages older than 28 days",
                oldMessages.Count);

            // Also update JSON file to remove old messages
            await UpdateJsonBackupAsync(cancellationToken);
        }
        else
        {
            _logger.LogInformation("Contact message cleanup: No messages older than 28 days found");
        }
    }

    private async Task UpdateJsonBackupAsync(CancellationToken cancellationToken)
    {
        try
        {
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "JsonData", "ContactMessagesDatabase.json");
            
            if (!File.Exists(jsonPath))
            {
                _logger.LogInformation("JSON backup file does not exist, skipping JSON cleanup");
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Get all non-deleted messages for JSON
            var activeMessages = await context.ContactMessages
                .Where(m => !m.IsDeleted)
                .Include(m => m.Event)
                .OrderByDescending(m => m.CreatedDate)
                .ToListAsync(cancellationToken);

            // Save to JSON
            var options = new System.Text.Json.JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = null,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            };
            var json = System.Text.Json.JsonSerializer.Serialize(activeMessages, options);
            await File.WriteAllTextAsync(jsonPath, json, cancellationToken);

            _logger.LogInformation("JSON backup updated successfully. Active messages: {Count}", activeMessages.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating JSON backup file");
        }
    }
}
