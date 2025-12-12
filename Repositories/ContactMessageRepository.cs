using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace MarutiTrainingPortal.Repositories
{
    public class ContactMessageRepository : IContactMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public ContactMessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(List<ContactMessage> messages, int totalCount)> GetPagedAsync(
            int page, 
            int pageSize, 
            string? searchQuery = null, 
            string? filter = "All")
        {
            var query = _context.ContactMessages
                .Include(m => m.Event)
                .AsQueryable();

            // Apply filter
            if (filter == "New")
            {
                query = query.Where(m => !m.IsRead);
            }

            // Apply search
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var search = searchQuery.ToLower().Trim();
                query = query.Where(m => 
                    m.Name.ToLower().Contains(search) ||
                    m.Email.ToLower().Contains(search) ||
                    m.Subject.ToLower().Contains(search) ||
                    (m.Message != null && m.Message.ToLower().Contains(search)));
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination and ordering
            var messages = await query
                .OrderByDescending(m => m.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (messages, totalCount);
        }

        public async Task<ContactMessage?> GetByIdAsync(int id)
        {
            return await _context.ContactMessages
                .Include(m => m.Event)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> MarkAsReadAsync(int id, bool isRead = true)
        {
            var message = await _context.ContactMessages
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null) return false;

            message.IsRead = isRead;
            message.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var message = await _context.ContactMessages
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null) return false;

            message.IsDeleted = true;
            message.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetNewCountAsync()
        {
            return await _context.ContactMessages
                .Where(m => !m.IsRead)
                .CountAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.ContactMessages
                .CountAsync();
        }

        public async Task<List<ContactMessage>> ExportAsync(string? filter = "All")
        {
            var query = _context.ContactMessages
                .Include(m => m.Event)
                .AsQueryable();

            if (filter == "New")
            {
                query = query.Where(m => !m.IsRead);
            }

            return await query
                .OrderByDescending(m => m.CreatedDate)
                .ToListAsync();
        }
    }
}
