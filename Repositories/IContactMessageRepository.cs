using MarutiTrainingPortal.Models;

namespace MarutiTrainingPortal.Repositories
{
    public interface IContactMessageRepository
    {
        /// <summary>
        /// Get paged list of contact messages with optional search and filter
        /// </summary>
        Task<(List<ContactMessage> messages, int totalCount)> GetPagedAsync(
            int page, 
            int pageSize, 
            string? searchQuery = null, 
            string? filter = "All");

        /// <summary>
        /// Get a single message by ID
        /// </summary>
        Task<ContactMessage?> GetByIdAsync(int id);

        /// <summary>
        /// Mark message as read or unread
        /// </summary>
        Task<bool> MarkAsReadAsync(int id, bool isRead = true);

        /// <summary>
        /// Soft delete a message
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Get count of new (unread) messages
        /// </summary>
        Task<int> GetNewCountAsync();

        /// <summary>
        /// Get total count of non-deleted messages
        /// </summary>
        Task<int> GetTotalCountAsync();

        /// <summary>
        /// Export messages to list for CSV generation
        /// </summary>
        Task<List<ContactMessage>> ExportAsync(string? filter = "All");
    }
}
