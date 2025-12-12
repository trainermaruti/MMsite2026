using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace MarutiTrainingPortal.Helpers
{
    /// <summary>
    /// Seeds the admin user from User Secrets (dev) or Environment Variables (production).
    /// NO CREDENTIALS ARE STORED IN CODE - they come from configuration only.
    /// </summary>
    public class AdminSeeder
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Read from User Secrets (dev) or Environment Variables (production)
            // Set via: dotnet user-secrets set "Admin:Email" "admin@marutitraining.com"
            // Set via: dotnet user-secrets set "Admin:Password" "YourSecurePassword123!"
            var adminEmail = configuration["Admin:Email"];
            var adminPassword = configuration["Admin:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                throw new InvalidOperationException(
                    "Admin credentials not configured! " +
                    "Set 'Admin:Email' and 'Admin:Password' via User Secrets (dev) or Environment Variables (prod). " +
                    "Run: dotnet user-secrets set \"Admin:Email\" \"your-email@example.com\" " +
                    "and: dotnet user-secrets set \"Admin:Password\" \"YourSecurePassword123!\""
                );
            }

            // Ensure Admin role exists
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Check if admin user already exists
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                // Create admin user
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true // Skip email confirmation for seeded admin
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                    );
                }

                // Assign Admin role
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
                // Ensure existing user has Admin role
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
