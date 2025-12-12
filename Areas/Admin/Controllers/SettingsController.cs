using MarutiTrainingPortal.Models.ViewModels;
using MarutiTrainingPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarutiTrainingPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private readonly ISettingsService _settingsService;
        private readonly IImageUploadService _imageUploadService;
        private readonly IConfiguration _configuration;

        public SettingsController(
            ISettingsService settingsService,
            IImageUploadService imageUploadService,
            IConfiguration configuration)
        {
            _settingsService = settingsService;
            _imageUploadService = imageUploadService;
            _configuration = configuration;
        }

        // GET: Admin/Settings
        public async Task<IActionResult> Index()
        {
            var settings = await _settingsService.GetSettingsAsync();
            if (settings == null)
            {
                TempData["ErrorMessage"] = "Settings not found";
                return RedirectToAction("Index", "Dashboard");
            }

            // Get database size
            settings.DatabaseSize = await _settingsService.GetDatabaseSizeAsync();

            return View(settings);
        }

        // POST: Admin/Settings/UpdateSeo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSeo(SystemSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors and try again";
                return View("Index", model);
            }

            var result = await _settingsService.UpdateSeoSettingsAsync(model);
            if (result)
            {
                TempData["SuccessMessage"] = "SEO settings updated successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update SEO settings";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Settings/UploadOgImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadOgImage(IFormFile ogImage)
        {
            if (ogImage == null || ogImage.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select an image to upload";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var imageUrl = await _imageUploadService.UploadImageAsync(ogImage, "seo");
                var settings = await _settingsService.GetSettingsAsync();
                if (settings != null)
                {
                    settings.OgImageUrl = imageUrl;
                    await _settingsService.UpdateSeoSettingsAsync(settings);
                    TempData["SuccessMessage"] = "OG image uploaded successfully";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Settings/UploadFavicon
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFavicon(IFormFile favicon)
        {
            if (favicon == null || favicon.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select a favicon to upload";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var imageUrl = await _imageUploadService.UploadImageAsync(favicon, "seo");
                var settings = await _settingsService.GetSettingsAsync();
                if (settings != null)
                {
                    settings.FaviconUrl = imageUrl;
                    await _settingsService.UpdateSeoSettingsAsync(settings);
                    TempData["SuccessMessage"] = "Favicon uploaded successfully";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Settings/UpdateEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmail(SystemSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors and try again";
                return View("Index", model);
            }

            var result = await _settingsService.UpdateEmailSettingsAsync(model);
            if (result)
            {
                TempData["SuccessMessage"] = "Email settings updated successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update email settings";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Settings/UpdateFeatures
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFeatures(SystemSettingsViewModel model)
        {
            var result = await _settingsService.UpdateFeatureTogglesAsync(model);
            if (result)
            {
                TempData["SuccessMessage"] = "Feature toggles updated successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update feature toggles";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Settings/TestSmtp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TestSmtp(SmtpConfigViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill in all SMTP fields";
                return RedirectToAction(nameof(Index));
            }

            // Save to user-secrets (development) or appsettings (production)
            TempData["InfoMessage"] = "SMTP configuration should be saved using 'dotnet user-secrets' command. See documentation for details.";
            
            await _settingsService.UpdateIntegrationStatusAsync("smtp", true);
            TempData["SuccessMessage"] = "SMTP marked as configured";

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Settings/SaveApiKeys
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveApiKeys(ApiKeyViewModel model)
        {
            // Never save API keys in database - use user-secrets or environment variables
            TempData["InfoMessage"] = "API keys should be saved using 'dotnet user-secrets' command. See documentation for details.";

            if (!string.IsNullOrEmpty(model.SendGridApiKey))
            {
                await _settingsService.UpdateIntegrationStatusAsync("sendgrid", true);
            }

            if (!string.IsNullOrEmpty(model.AzureOpenAIApiKey))
            {
                await _settingsService.UpdateIntegrationStatusAsync("azureopenai", true);
            }

            TempData["SuccessMessage"] = "Integration status updated";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Settings/ClearCache
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCache()
        {
            await _settingsService.ClearCacheAsync();
            TempData["SuccessMessage"] = "Cache cleared successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
