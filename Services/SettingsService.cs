using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;
using MarutiTrainingPortal.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MarutiTrainingPortal.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private const string SETTINGS_CACHE_KEY = "SystemSettings";

        public SettingsService(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<SystemSettingsViewModel?> GetSettingsAsync()
        {
            // Try to get from cache first
            if (_cache.TryGetValue(SETTINGS_CACHE_KEY, out SystemSettingsViewModel? cached))
            {
                return cached;
            }

            var settings = await _context.SystemSettings.FirstOrDefaultAsync();
            if (settings == null) return null;

            var viewModel = new SystemSettingsViewModel
            {
                Id = settings.Id,
                SiteTitle = settings.SiteTitle,
                MetaDescription = settings.MetaDescription,
                MetaKeywords = settings.MetaKeywords,
                OgImageUrl = settings.OgImageUrl,
                FaviconUrl = settings.FaviconUrl,
                ContactFormReceiverEmail = settings.ContactFormReceiverEmail,
                SecondaryNotificationEmail = settings.SecondaryNotificationEmail,
                EmailNotificationsEnabled = settings.EmailNotificationsEnabled,
                MaintenanceMode = settings.MaintenanceMode,
                ShowUpcomingEvents = settings.ShowUpcomingEvents,
                ShowCoursesSection = settings.ShowCoursesSection,
                ShowProfileSection = settings.ShowProfileSection,
                SmtpConfigured = settings.SmtpConfigured,
                SendGridConfigured = settings.SendGridConfigured,
                AzureOpenAIConfigured = settings.AzureOpenAIConfigured,
                AppVersion = settings.AppVersion,
                LastBackupDate = settings.LastBackupDate,
                DatabaseSize = settings.DatabaseSize,
                UpdatedDate = settings.UpdatedDate
            };

            // Cache for 5 minutes
            _cache.Set(SETTINGS_CACHE_KEY, viewModel, TimeSpan.FromMinutes(5));

            return viewModel;
        }

        public async Task<bool> UpdateSeoSettingsAsync(SystemSettingsViewModel model)
        {
            var settings = await _context.SystemSettings.FirstOrDefaultAsync();
            if (settings == null) return false;

            settings.SiteTitle = model.SiteTitle;
            settings.MetaDescription = model.MetaDescription;
            settings.MetaKeywords = model.MetaKeywords;
            settings.OgImageUrl = model.OgImageUrl;
            settings.FaviconUrl = model.FaviconUrl;
            settings.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _cache.Remove(SETTINGS_CACHE_KEY);
            return true;
        }

        public async Task<bool> UpdateEmailSettingsAsync(SystemSettingsViewModel model)
        {
            var settings = await _context.SystemSettings.FirstOrDefaultAsync();
            if (settings == null) return false;

            settings.ContactFormReceiverEmail = model.ContactFormReceiverEmail;
            settings.SecondaryNotificationEmail = model.SecondaryNotificationEmail;
            settings.EmailNotificationsEnabled = model.EmailNotificationsEnabled;
            settings.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _cache.Remove(SETTINGS_CACHE_KEY);
            return true;
        }

        public async Task<bool> UpdateFeatureTogglesAsync(SystemSettingsViewModel model)
        {
            var settings = await _context.SystemSettings.FirstOrDefaultAsync();
            if (settings == null) return false;

            settings.MaintenanceMode = model.MaintenanceMode;
            settings.ShowUpcomingEvents = model.ShowUpcomingEvents;
            settings.ShowCoursesSection = model.ShowCoursesSection;
            settings.ShowProfileSection = model.ShowProfileSection;
            settings.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _cache.Remove(SETTINGS_CACHE_KEY);
            return true;
        }

        public async Task<bool> UpdateIntegrationStatusAsync(string integrationType, bool isConfigured)
        {
            var settings = await _context.SystemSettings.FirstOrDefaultAsync();
            if (settings == null) return false;

            switch (integrationType.ToLower())
            {
                case "smtp":
                    settings.SmtpConfigured = isConfigured;
                    break;
                case "sendgrid":
                    settings.SendGridConfigured = isConfigured;
                    break;
                case "azureopenai":
                    settings.AzureOpenAIConfigured = isConfigured;
                    break;
                default:
                    return false;
            }

            settings.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _cache.Remove(SETTINGS_CACHE_KEY);
            return true;
        }

        public async Task<string> GetDatabaseSizeAsync()
        {
            try
            {
                // Get approximate database size
                var tableNames = new[] { "Trainings", "Courses", "TrainingEvents", "Profiles", "ContactMessages", "Certificates" };
                long totalRows = 0;

                foreach (var table in tableNames)
                {
                    var count = table switch
                    {
                        "Trainings" => await _context.Trainings.CountAsync(),
                        "Courses" => await _context.Courses.CountAsync(),
                        "TrainingEvents" => await _context.TrainingEvents.CountAsync(),
                        "Profiles" => await _context.Profiles.CountAsync(),
                        "ContactMessages" => await _context.ContactMessages.CountAsync(),
                        "Certificates" => await _context.Certificates.CountAsync(),
                        _ => 0
                    };
                    totalRows += count;
                }

                // Rough estimate: average 1KB per row
                double sizeInMB = (totalRows * 1024.0) / (1024 * 1024);
                return $"{sizeInMB:F2} MB (approx)";
            }
            catch
            {
                return "N/A";
            }
        }

        public Task ClearCacheAsync()
        {
            _cache.Remove(SETTINGS_CACHE_KEY);
            return Task.CompletedTask;
        }
    }
}
