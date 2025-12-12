using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;
using MarutiTrainingPortal.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MarutiTrainingPortal.Services
{
    public class ProfileService : IProfileService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<AdminProfileViewModel?> GetAdminProfileAsync()
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync();
            if (profile == null) return null;

            return new AdminProfileViewModel
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
                TwitterUrl = profile.TwitterUrl,
                GitHubUrl = profile.GitHubUrl,
                UpdatedDate = profile.UpdatedDate
            };
        }

        public async Task<bool> UpdatePersonalInfoAsync(AdminProfileViewModel model)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync();
            if (profile == null) return false;

            // Sanitize HTML in Bio
            profile.FullName = model.FullName;
            profile.Title = model.Title;
            profile.Bio = SanitizeHtml(model.Bio);
            profile.Expertise = model.Expertise;
            profile.CertificationsAndAchievements = model.CertificationsAndAchievements;
            profile.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateContactInfoAsync(AdminProfileViewModel model)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync();
            if (profile == null) return false;

            profile.Email = model.Email;
            profile.PhoneNumber = model.PhoneNumber;
            profile.WhatsAppNumber = model.WhatsAppNumber;
            profile.LinkedInUrl = model.LinkedInUrl;
            profile.InstagramUrl = model.InstagramUrl;
            profile.YouTubeUrl = model.YouTubeUrl;
            profile.SkillTechUrl = model.SkillTechUrl;
            profile.TwitterUrl = model.TwitterUrl;
            profile.GitHubUrl = model.GitHubUrl;
            profile.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProfilePhotoAsync(string imageUrl)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync();
            if (profile == null) return false;

            profile.ProfileImageUrl = imageUrl;
            profile.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordViewModel model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            return result.Succeeded;
        }

        private string SanitizeHtml(string html)
        {
            if (string.IsNullOrEmpty(html)) return html;

            // Basic sanitization - remove script tags and dangerous attributes
            html = System.Text.RegularExpressions.Regex.Replace(html, @"<script[^>]*>.*?</script>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            html = System.Text.RegularExpressions.Regex.Replace(html, @"on\w+\s*=", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            html = System.Text.RegularExpressions.Regex.Replace(html, @"javascript:", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            return html;
        }
    }
}
