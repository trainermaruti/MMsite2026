using MarutiTrainingPortal.Models;

namespace MarutiTrainingPortal.Services
{
    public interface IMessageCleanupService
    {
        /// <summary>
        /// Automatically soft-delete messages older than 28 days
        /// </summary>
        Task<int> DeleteExpiredMessagesAsync(int daysOld = 28);

        /// <summary>
        /// Manually soft-delete a specific message
        /// </summary>
        Task<bool> DeleteMessageAsync(int messageId);

        /// <summary>
        /// Get count of messages that will be auto-deleted
        /// </summary>
        Task<int> GetExpiredMessageCountAsync(int daysOld = 28);

        /// <summary>
        /// Get list of messages nearing expiration (21+ days old)
        /// </summary>
        Task<List<ContactMessage>> GetExpiringMessagesAsync(int daysOld = 21);
    }
}
