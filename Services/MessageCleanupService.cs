using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace MarutiTrainingPortal.Services
{
    public class MessageCleanupService : IMessageCleanupService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MessageCleanupService> _logger;

        public MessageCleanupService(ApplicationDbContext context, ILogger<MessageCleanupService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Automatically soft-delete messages older than specified days
        /// </summary>
        public async Task<int> DeleteExpiredMessagesAsync(int daysOld = 28)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
                
                var expiredMessages = await _context.ContactMessages
                    .IgnoreQueryFilters()
                    .Where(m => m.CreatedDate < cutoffDate && !m.IsDeleted)
                    .ToListAsync();

                if (expiredMessages.Count == 0)
                {
                    _logger.LogInformation("No expired messages to delete (threshold: {DaysOld} days old)", daysOld);
                    return 0;
                }

                foreach (var message in expiredMessages)
                {
                    message.IsDeleted = true;
                    message.UpdatedDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted {Count} expired messages older than {DaysOld} days", expiredMessages.Count, daysOld);
                
                return expiredMessages.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting expired messages: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Manually soft-delete a specific message
        /// </summary>
        public async Task<bool> DeleteMessageAsync(int messageId)
        {
            try
            {
                var message = await _context.ContactMessages
                    .FirstOrDefaultAsync(m => m.Id == messageId);

                if (message == null)
                {
                    _logger.LogWarning("Message with ID {MessageId} not found for deletion", messageId);
                    return false;
                }

                message.IsDeleted = true;
                message.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Message {MessageId} manually deleted", messageId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting message {MessageId}: {ErrorMessage}", messageId, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Get count of messages that will be auto-deleted
        /// </summary>
        public async Task<int> GetExpiredMessageCountAsync(int daysOld = 28)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
                
                return await _context.ContactMessages
                    .IgnoreQueryFilters()
                    .Where(m => m.CreatedDate < cutoffDate && !m.IsDeleted)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting expired messages: {ErrorMessage}", ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// Get list of messages nearing expiration (21+ days old)
        /// </summary>
        public async Task<List<ContactMessage>> GetExpiringMessagesAsync(int daysOld = 21)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
                
                return await _context.ContactMessages
                    .Where(m => m.CreatedDate < cutoffDate)
                    .OrderBy(m => m.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expiring messages: {ErrorMessage}", ex.Message);
                return new List<ContactMessage>();
            }
        }
    }
}
