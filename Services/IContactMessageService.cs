using MarutiTrainingPortal.Models;

namespace MarutiTrainingPortal.Services
{
    public interface IContactMessageService
    {
        Task<(List<ContactMessage> messages, int totalCount, int newCount, int totalMessages)> GetInboxAsync(
            int page, 
            int pageSize, 
            string? searchQuery = null, 
            string? filter = "All");

        Task<ContactMessage?> GetMessageAsync(int id);

        Task<bool> MarkAsReadAsync(int id, bool isRead = true);

        Task<bool> DeleteMessageAsync(int id);

        Task<byte[]> ExportToCsvAsync(string? filter = "All");

        Task<List<ContactMessage>> GetAllMessagesForBackupAsync();
    }
}
