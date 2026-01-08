using MarutiTrainingPortal.Models;
using MarutiTrainingPortal.Models.ViewModels;
using MarutiTrainingPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MarutiTrainingPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly IImageUploadService _imageUploadService;
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileController(
            IProfileService profileService,
            IImageUploadService imageUploadService,
            UserManager<IdentityUser> userManager)
        {
            _profileService = profileService;
            _imageUploadService = imageUploadService;
            _userManager = userManager;
        }

        // GET: Admin/Profile
        public async Task<IActionResult> Index()
        {
            try
            {
                // Read profile from JSON file
                var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "JsonData", "ProfilesDatabase.json");
                var jsonData = await System.IO.File.ReadAllTextAsync(jsonPath);
                var profiles = System.Text.Json.JsonSerializer.Deserialize<List<Profile>>(jsonData);

                if (profiles == null || !profiles.Any())
                {
                    TempData["ErrorMessage"] = "Profile not found";
                    return RedirectToAction("Index", "Dashboard");
                }

                var profile = profiles.First();

                // Map to ViewModel
                var model = new AdminProfileViewModel
                {
                    Id = profile.Id,
                    FullName = profile.FullName,
                    Title = profile.Title,
                    Bio = profile.Bio,
                    Expertise = profile.Expertise,
                    CertificationsAndAchievements = profile.CertificationsAndAchievements,
                    ProfileImageUrl = profile.ProfileImageUrl,
                    Email = profile.Email,
                    PhoneNumber = profile.PhoneNumber,
                    WhatsAppNumber = profile.WhatsAppNumber,
                    LinkedInUrl = profile.LinkedInUrl,
                    InstagramUrl = profile.InstagramUrl,
                    YouTubeUrl = profile.YouTubeUrl,
                    SkillTechUrl = profile.SkillTechUrl,
                    UpdatedDate = profile.UpdatedDate
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading profile: {ex.Message}";
                return RedirectToAction("Index", "Dashboard");
            }
        }

        // POST: Admin/Profile/UpdatePersonalInfo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePersonalInfo(AdminProfileViewModel model)
        {
            // Remove Contact & Social fields from validation since they're not in this form
            ModelState.Remove("Email");
            
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors and try again";
                return View("Index", model);
            }

            try
            {
                // Read current JSON file
                var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "JsonData", "ProfilesDatabase.json");
                var jsonData = await System.IO.File.ReadAllTextAsync(jsonPath);
                var profiles = System.Text.Json.JsonSerializer.Deserialize<List<Profile>>(jsonData);

                if (profiles == null || !profiles.Any()) return RedirectToAction(nameof(Index));
                
                var profile = profiles.First();
                
                // Update profile with new data
                profile.FullName = model.FullName;
                profile.Title = model.Title;
                profile.Bio = model.Bio;
                profile.Expertise = model.Expertise;
                profile.CertificationsAndAchievements = model.CertificationsAndAchievements;
                profile.UpdatedDate = DateTime.UtcNow;

                // Write back to JSON as array
                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                var updatedJson = System.Text.Json.JsonSerializer.Serialize(profiles, options);
                await System.IO.File.WriteAllTextAsync(jsonPath, updatedJson);

                TempData["SuccessMessage"] = "Profile synced to JSON successfully";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to sync profile: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Profile/UpdateContactInfo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateContactInfo(AdminProfileViewModel model)
        {
            // Remove Personal Info fields from validation since they're not in this form
            ModelState.Remove("FullName");
            ModelState.Remove("Title");
            ModelState.Remove("Bio");
            ModelState.Remove("Expertise");
            
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors and try again";
                return View("Index", model);
            }

            try
            {
                // Read current JSON file
                var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "JsonData", "ProfilesDatabase.json");
                var jsonData = await System.IO.File.ReadAllTextAsync(jsonPath);
                var profiles = System.Text.Json.JsonSerializer.Deserialize<List<Profile>>(jsonData);

                if (profiles == null || !profiles.Any()) return RedirectToAction(nameof(Index));
                
                var profile = profiles.First();
                
                // Update contact info with new data
                profile.Email = model.Email;
                profile.PhoneNumber = model.PhoneNumber;
                profile.WhatsAppNumber = model.WhatsAppNumber;
                profile.LinkedInUrl = model.LinkedInUrl;
                profile.InstagramUrl = model.InstagramUrl;
                profile.YouTubeUrl = model.YouTubeUrl;
                profile.SkillTechUrl = model.SkillTechUrl;
                profile.UpdatedDate = DateTime.UtcNow;

                // Write back to JSON as array
                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                var updatedJson = System.Text.Json.JsonSerializer.Serialize(profiles, options);
                await System.IO.File.WriteAllTextAsync(jsonPath, updatedJson);

                TempData["SuccessMessage"] = "Contact information synced to JSON successfully";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to sync contact info: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Profile/UploadPhoto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPhoto(IFormFile profileImage)
        {
            if (profileImage == null || profileImage.Length == 0)
            {
                TempData["ErrorMessage"] = "Please select an image to upload";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var imageUrl = await _imageUploadService.UploadImageAsync(profileImage, "profiles");
                
                // Sync to JSON
                var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "JsonData", "ProfilesDatabase.json");
                var jsonData = await System.IO.File.ReadAllTextAsync(jsonPath);
                var profiles = System.Text.Json.JsonSerializer.Deserialize<List<Profile>>(jsonData);
                
                if (profiles == null || !profiles.Any()) return RedirectToAction(nameof(Index));
                
                var profile = profiles.First();
                profile.ProfileImageUrl = imageUrl;
                profile.UpdatedDate = DateTime.UtcNow;
                
                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
                var updatedJson = System.Text.Json.JsonSerializer.Serialize(profiles, options);
                await System.IO.File.WriteAllTextAsync(jsonPath, updatedJson);

                TempData["SuccessMessage"] = "Profile photo synced to JSON successfully";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors and try again";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction(nameof(Index));
            }

            var result = await _profileService.ChangePasswordAsync(user.Id, model);
            if (result)
            {
                TempData["SuccessMessage"] = "Password changed successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to change password. Please check your current password";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
