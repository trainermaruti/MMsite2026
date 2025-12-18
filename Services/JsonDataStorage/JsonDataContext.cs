using MarutiTrainingPortal.Models;
using MarutiTrainingPortal.Services.JsonDataStorage.Repositories;

namespace MarutiTrainingPortal.Services.JsonDataStorage
{
    /// <summary>
    /// JSON-based data context that mimics ApplicationDbContext for easier migration
    /// </summary>
    public class JsonDataContext
    {
        public CoursesJsonRepository Courses { get; }
        public TrainingsJsonRepository Trainings { get; }
        public EventsJsonRepository TrainingEvents { get; }
        public EventRegistrationsJsonRepository TrainingEventRegistrations { get; }
        public ContactMessagesJsonRepository ContactMessages { get; }
        public CertificatesJsonRepository Certificates { get; }
        public ProfilesJsonRepository Profiles { get; }
        public FeaturedVideosJsonRepository FeaturedVideos { get; }
        public WebsiteImagesJsonRepository WebsiteImages { get; }
        public SystemSettingsJsonRepository SystemSettings { get; }
        public LeadAuditLogsJsonRepository LeadAuditLogs { get; }

        public JsonDataContext(JsonFileService jsonFileService)
        {
            Courses = new CoursesJsonRepository(jsonFileService);
            Trainings = new TrainingsJsonRepository(jsonFileService);
            TrainingEvents = new EventsJsonRepository(jsonFileService);
            TrainingEventRegistrations = new EventRegistrationsJsonRepository(jsonFileService);
            ContactMessages = new ContactMessagesJsonRepository(jsonFileService);
            Certificates = new CertificatesJsonRepository(jsonFileService);
            Profiles = new ProfilesJsonRepository(jsonFileService);
            FeaturedVideos = new FeaturedVideosJsonRepository(jsonFileService);
            WebsiteImages = new WebsiteImagesJsonRepository(jsonFileService);
            SystemSettings = new SystemSettingsJsonRepository(jsonFileService);
            LeadAuditLogs = new LeadAuditLogsJsonRepository(jsonFileService);
        }

        /// <summary>
        /// Saves all changes - for API compatibility (actual saves happen immediately in JSON repos)
        /// </summary>
        public Task<int> SaveChangesAsync()
        {
            // JSON repositories save immediately, so this just returns success
            return Task.FromResult(1);
        }
    }
}
