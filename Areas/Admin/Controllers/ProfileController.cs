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
            var profile = await _profileService.GetAdminProfileAsync();
            if (profile == null)
            {
                TempData["ErrorMessage"] = "Profile not found";
                return RedirectToAction("Index", "Dashboard");
            }

            return View(profile);
        }

        // POST: Admin/Profile/UpdatePersonalInfo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePersonalInfo(AdminProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors and try again";
                return View("Index", model);
            }

            var result = await _profileService.UpdatePersonalInfoAsync(model);
            if (result)
            {
                TempData["SuccessMessage"] = "Personal information updated successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update personal information";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Profile/UpdateContactInfo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateContactInfo(AdminProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors and try again";
                return View("Index", model);
            }

            var result = await _profileService.UpdateContactInfoAsync(model);
            if (result)
            {
                TempData["SuccessMessage"] = "Contact information updated successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update contact information";
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
                var result = await _profileService.UpdateProfilePhotoAsync(imageUrl);

                if (result)
                {
                    TempData["SuccessMessage"] = "Profile photo updated successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update profile photo";
                }
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
