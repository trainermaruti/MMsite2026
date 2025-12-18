using MarutiTrainingPortal.Models;

namespace MarutiTrainingPortal.Services.JsonDataStorage.Repositories
{
    public class CoursesJsonRepository : JsonRepository<Course>
    {
        public CoursesJsonRepository(JsonFileService jsonFileService) 
            : base(jsonFileService, "CoursesDatabase.json")
        {
        }

        public async Task<List<Course>> GetByCategory(string category)
        {
            var all = await GetAllAsync();
            return all.Where(c => !c.IsDeleted && c.Category == category).ToList();
        }

        public async Task<List<Course>> GetByLevel(string level)
        {
            var all = await GetAllAsync();
            return all.Where(c => !c.IsDeleted && c.Level == level).ToList();
        }

        public async Task<List<Course>> GetPublishedCourses()
        {
            var all = await GetAllAsync();
            return all.Where(c => !c.IsDeleted).OrderByDescending(c => c.PublishedDate).ToList();
        }
    }

    public class TrainingsJsonRepository : JsonRepository<Training>
    {
        public TrainingsJsonRepository(JsonFileService jsonFileService) 
            : base(jsonFileService, "TrainingsDatabase.json")
        {
        }

        public async Task<List<Training>> GetUpcomingTrainings()
        {
            var all = await GetAllAsync();
            return all.Where(t => !t.IsDeleted && t.DeliveryDate >= DateTime.Now)
                     .OrderBy(t => t.DeliveryDate)
                     .ToList();
        }

        public async Task<List<Training>> GetPastTrainings()
        {
            var all = await GetAllAsync();
            return all.Where(t => !t.IsDeleted && t.DeliveryDate < DateTime.Now)
                     .OrderByDescending(t => t.DeliveryDate)
                     .ToList();
        }
    }

    public class EventsJsonRepository : JsonRepository<TrainingEvent>
    {
        public EventsJsonRepository(JsonFileService jsonFileService) 
            : base(jsonFileService, "EventsDatabase.json")
        {
        }

        public async Task<List<TrainingEvent>> GetUpcomingEvents()
        {
            var all = await GetAllAsync();
            return all.Where(e => !e.IsDeleted && e.StartDate >= DateTime.Now)
                     .OrderBy(e => e.StartDate)
                     .ToList();
        }

        public async Task<List<TrainingEvent>> GetFeaturedEvents()
        {
            var all = await GetAllAsync();
            return all.Where(e => !e.IsDeleted && e.StartDate >= DateTime.Now)
                     .OrderBy(e => e.StartDate)
                     .Take(5) // Get first 5 as featured
                     .ToList();
        }
    }

    public class EventRegistrationsJsonRepository : JsonRepository<TrainingEventRegistration>
    {
        public EventRegistrationsJsonRepository(JsonFileService jsonFileService) 
            : base(jsonFileService, "EventRegistrationsDatabase.json")
        {
        }

        public async Task<List<TrainingEventRegistration>> GetByEventId(int eventId)
        {
            var all = await GetAllAsync();
            return all.Where(r => r.TrainingEventId == eventId).ToList();
        }

        public async Task<int> GetEventRegistrationCount(int eventId)
        {
            var all = await GetAllAsync();
            return all.Count(r => r.TrainingEventId == eventId);
        }
    }

    public class ContactMessagesJsonRepository : JsonRepository<ContactMessage>
    {
        public ContactMessagesJsonRepository(JsonFileService jsonFileService) 
            : base(jsonFileService, "ContactMessagesDatabase.json")
        {
        }

        public async Task<List<ContactMessage>> GetUnreadMessages()
        {
            var all = await GetAllAsync();
            return all.Where(m => !m.IsDeleted && !m.IsRead)
                     .OrderByDescending(m => m.CreatedDate)
                     .ToList();
        }

        public async Task<List<ContactMessage>> GetByStatus(string status)
        {
            var all = await GetAllAsync();
            return all.Where(m => !m.IsDeleted && m.Status == status)
                     .OrderByDescending(m => m.CreatedDate)
                     .ToList();
        }

        public async Task<int> GetUnreadCount()
        {
            var all = await GetAllAsync();
            return all.Count(m => !m.IsDeleted && !m.IsRead);
        }
    }

    public class CertificatesJsonRepository : JsonRepository<Certificate>
    {
        public CertificatesJsonRepository(JsonFileService jsonFileService) 
            : base(jsonFileService, "CertificatesDatabase.json")
        {
        }

        public async Task<Certificate?> GetByCertificateId(string certificateId)
        {
            var all = await GetAllAsync();
            return all.FirstOrDefault(c => !c.IsDeleted && c.CertificateId == certificateId);
        }

        public async Task<List<Certificate>> GetByStudentEmail(string email)
        {
            var all = await GetAllAsync();
            return all.Where(c => !c.IsDeleted && c.StudentEmail == email).ToList();
        }
    }

    public class ProfilesJsonRepository : JsonRepository<Profile>
    {
        public ProfilesJsonRepository(JsonFileService jsonFileService) 
            : base(jsonFileService, "ProfilesDatabase.json")
        {
        }

        public async Task<Profile?> GetFirstProfile()
        {
            var all = await GetAllAsync();
            return all.FirstOrDefault();
        }
    }

    public class FeaturedVideosJsonRepository : JsonRepository<FeaturedVideo>
    {
        public FeaturedVideosJsonRepository(JsonFileService jsonFileService) 
            : base(jsonFileService, "FeaturedVideosDatabase.json")
        {
        }

        public async Task<FeaturedVideo?> GetActiveVideo()
        {
            var all = await GetAllAsync();
            return all.FirstOrDefault(v => !v.IsDeleted && v.IsActive);
        }
    }

    public class WebsiteImagesJsonRepository : JsonRepository<WebsiteImage>
    {
        public WebsiteImagesJsonRepository(JsonFileService jsonFileService) 
            : base(jsonFileService, "ImagesDatabase.json")
        {
        }

        public async Task<WebsiteImage?> GetByImageKey(string imageKey)
        {
            var all = await GetAllAsync();
            return all.FirstOrDefault(i => !i.IsDeleted && i.ImageKey == imageKey);
        }

        public async Task<List<WebsiteImage>> GetByCategory(string category)
        {
            var all = await GetAllAsync();
            return all.Where(i => !i.IsDeleted && i.Category == category).ToList();
        }
    }

    public class SystemSettingsJsonRepository : JsonRepository<SystemSettings>
    {
        public SystemSettingsJsonRepository(JsonFileService jsonFileService) 
            : base(jsonFileService, "SystemSettingsDatabase.json")
        {
        }

        public async Task<SystemSettings?> GetSettings()
        {
            var all = await GetAllAsync();
            return all.FirstOrDefault();
        }
    }

    public class LeadAuditLogsJsonRepository : JsonRepository<LeadAuditLog>
    {
        public LeadAuditLogsJsonRepository(JsonFileService jsonFileService) 
            : base(jsonFileService, "LeadAuditLogsDatabase.json")
        {
        }

        public async Task<List<LeadAuditLog>> GetByContactMessageId(int contactMessageId)
        {
            var all = await GetAllAsync();
            return all.Where(l => l.ContactMessageId == contactMessageId)
                     .OrderByDescending(l => l.Id) // Order by Id instead of Timestamp
                     .ToList();
        }
    }
}
