using MarutiTrainingPortal.Models;
using MarutiTrainingPortal.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace MarutiTrainingPortal.Services
{
    public interface IProfileService
    {
        Task<AdminProfileViewModel?> GetAdminProfileAsync();
        Task<bool> UpdatePersonalInfoAsync(AdminProfileViewModel model);
        Task<bool> UpdateContactInfoAsync(AdminProfileViewModel model);
        Task<bool> UpdateProfilePhotoAsync(string imageUrl);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordViewModel model);
    }

    public interface ISettingsService
    {
        Task<SystemSettingsViewModel?> GetSettingsAsync();
        Task<bool> UpdateSeoSettingsAsync(SystemSettingsViewModel model);
        Task<bool> UpdateEmailSettingsAsync(SystemSettingsViewModel model);
        Task<bool> UpdateFeatureTogglesAsync(SystemSettingsViewModel model);
        Task<bool> UpdateIntegrationStatusAsync(string integrationType, bool isConfigured);
        Task<string> GetDatabaseSizeAsync();
        Task ClearCacheAsync();
    }

    public interface IImageUploadService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder);
        Task<bool> DeleteImageAsync(string imageUrl);
        string GetImagePath(string imageUrl);
    }
}
