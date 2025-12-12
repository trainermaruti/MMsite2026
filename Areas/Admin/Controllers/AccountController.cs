using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MarutiTrainingPortal.Models.ViewModels;

namespace MarutiTrainingPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // Lockout: 15 minutes after 5 failed attempts
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true
                );

                if (result.Succeeded)
                {
                    // Verify user has Admin role (extra security check)
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        // Redirect to admin dashboard or requested URL
                        return RedirectToLocal(returnUrl ?? Url.Action("Index", "Dashboard", new { area = "Admin" }));
                    }
                    else
                    {
                        await _signInManager.SignOutAsync();
                        ModelState.AddModelError(string.Empty, "You do not have admin privileges.");
                        return View(model);
                    }
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Account locked due to multiple failed login attempts. Try again in 15 minutes.");
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { area = "" }); // Return to public home
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }
    }
}
