using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;
using MarutiTrainingPortal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarutiTrainingPortal.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHtmlSanitizerService _htmlSanitizer;

        public ProfileController(ApplicationDbContext context, IHtmlSanitizerService htmlSanitizer)
        {
            _context = context;
            _htmlSanitizer = htmlSanitizer;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync();
            if (profile == null)
            {
                // Return a default profile if none exists
                profile = new Profile
                {
                    FullName = "Maruti Makwana",
                    Title = "Azure Expert & Trainer",
                    Bio = "Welcome! Profile information is being set up.",
                    Email = "info@example.com",
                    PhoneNumber = "+91-0000000000",
                    WhatsAppNumber = "+91-0000000000",
                    ProfileImageUrl = "/images/logo.jpg",
                    Expertise = "Azure, Cloud Computing",
                    TotalTrainingsDone = 0,
                    TotalStudents = 0,
                    LinkedInUrl = "#",
                    InstagramUrl = "#",
                    YouTubeUrl = "#",
                    SkillTechUrl = "#",
                    CertificationsAndAchievements = "Setting up..."
                };
            }

            return View(profile);
        }

        [AllowAnonymous]
        public async Task<IActionResult> About()
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync();
            if (profile == null)
            {
                // Return a default profile if none exists
                profile = new Profile
                {
                    FullName = "Maruti Makwana",
                    Title = "Azure Expert & Trainer",
                    Bio = "Welcome! Profile information is being set up.",
                    Email = "info@example.com",
                    PhoneNumber = "+91-0000000000",
                    WhatsAppNumber = "+91-0000000000",
                    ProfileImageUrl = "/images/logo.jpg",
                    Expertise = "Azure, Cloud Computing",
                    TotalTrainingsDone = 0,
                    TotalStudents = 0,
                    LinkedInUrl = "#",
                    InstagramUrl = "#",
                    YouTubeUrl = "#",
                    SkillTechUrl = "#",
                    CertificationsAndAchievements = "Setting up..."
                };
            }

            return View(profile);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit()
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync();
            if (profile == null)
                return NotFound();

            return View(profile);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Profile profile)
        {
            // Server-side role check
            if (!User.IsInRole("Admin"))
                return Forbid();

            if (ModelState.IsValid)
            {
                // Sanitize HTML input before saving
                profile.Bio = _htmlSanitizer.Sanitize(profile.Bio);

                profile.UpdatedDate = DateTime.Now;
                _context.Profiles.Update(profile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(profile);
        }

        [HttpPost]
        public IActionResult Contact(string email, string message)
        {
            // Redirect to contact page with message
            return RedirectToAction("Create", "Contact");
        }
    }
}
