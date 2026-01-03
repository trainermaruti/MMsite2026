using MarutiTrainingPortal.Models;
using MarutiTrainingPortal.Repositories;
using System.Text;

namespace MarutiTrainingPortal.Services
{
    public class ContactMessageService : IContactMessageService
    {
        private readonly IContactMessageRepository _repository;

        public ContactMessageService(IContactMessageRepository repository)
        {
            _repository = repository;
        }

        public async Task<(List<ContactMessage> messages, int totalCount, int newCount, int totalMessages)> GetInboxAsync(
            int page, 
            int pageSize, 
            string? searchQuery = null, 
            string? filter = "All")
        {
            var (messages, totalCount) = await _repository.GetPagedAsync(page, pageSize, searchQuery, filter);
            var newCount = await _repository.GetNewCountAsync();
            var totalMessages = await _repository.GetTotalCountAsync();

            return (messages, totalCount, newCount, totalMessages);
        }

        public async Task<ContactMessage?> GetMessageAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<bool> MarkAsReadAsync(int id, bool isRead = true)
        {
            return await _repository.MarkAsReadAsync(id, isRead);
        }

        public async Task<bool> DeleteMessageAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<byte[]> ExportToCsvAsync(string? filter = "All")
        {
            var messages = await _repository.ExportAsync(filter);

            var csv = new StringBuilder();
            
            // CSV Header
            csv.AppendLine("Id,Name,Email,PhoneNumber,Subject,Message,CreatedAt,IsRead,Status,EventId");

            // CSV Data
            foreach (var msg in messages)
            {
                csv.AppendLine($"{msg.Id}," +
                    $"\"{EscapeCsv(msg.Name)}\"," +
                    $"\"{EscapeCsv(msg.Email)}\"," +
                    $"\"{EscapeCsv(msg.PhoneNumber ?? "")}\"," +
                    $"\"{EscapeCsv(msg.Subject)}\"," +
                    $"\"{EscapeCsv(msg.Message)}\"," +
                    $"\"{msg.CreatedDate:yyyy-MM-dd HH:mm:ss}\"," +
                    $"{msg.IsRead}," +
                    $"\"{EscapeCsv(msg.Status)}\"," +
                    $"{msg.EventId?.ToString() ?? ""}");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            
            // Escape double quotes by doubling them
            return value.Replace("\"", "\"\"");
        }

        public async Task<List<ContactMessage>> GetAllMessagesForBackupAsync()
        {
            return await _repository.GetAllForBackupAsync();
        }
    }
}
