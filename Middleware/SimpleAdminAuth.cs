using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MarutiTrainingPortal.Middleware
{
    /// <summary>
    /// Lightweight alternative to ASP.NET Core Identity for admin authentication.
    /// This is a simpler implementation with fewer features than Identity.
    /// 
    /// TRADEOFFS:
    /// ✅ Pros: Simpler, less overhead, easier to understand, no database migrations for Identity tables
    /// ❌ Cons: No built-in lockout, email confirmation, role management, password reset, two-factor auth
    /// 
    /// USE CASES:
    /// - Simple admin-only sites with 1-2 admin users
    /// - When you don't need advanced Identity features
    /// - When database schema is tightly controlled
    /// 
    /// SECURITY FEATURES:
    /// - PBKDF2 password hashing (100,000 iterations)
    /// - Salted hashes (16-byte random salt)
    /// - Timing-attack resistant password comparison
    /// - Secure cookie authentication
    /// </summary>
    public class SimpleAdminAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SimpleAdminAuthMiddleware> _logger;

        public SimpleAdminAuthMiddleware(
            RequestDelegate next,
            IConfiguration configuration,
            ILogger<SimpleAdminAuthMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only intercept /Admin/Account/Login requests
            if (context.Request.Path.StartsWithSegments("/Admin/Account/Login"))
            {
                if (context.Request.Method == "POST")
                {
                    await HandleLoginAsync(context);
                    return;
                }
            }
            else if (context.Request.Path.StartsWithSegments("/Admin/Account/Logout"))
            {
                await HandleLogoutAsync(context);
                return;
            }
            else if (context.Request.Path.StartsWithSegments("/Admin"))
            {
                // Check if user is authenticated
                if (!context.User.Identity?.IsAuthenticated ?? true)
                {
                    context.Response.Redirect("/Admin/Account/Login");
                    return;
                }
            }

            await _next(context);
        }

        private async Task HandleLoginAsync(HttpContext context)
        {
            var form = await context.Request.ReadFormAsync();
            var email = form["Email"].ToString();
            var password = form["Password"].ToString();

            // Get credentials from configuration (User Secrets or environment variables)
            var adminEmail = _configuration["Admin:Email"];
            var adminPasswordHash = _configuration["Admin:PasswordHash"];

            // If PasswordHash not set, generate it from Password (for initial setup)
            if (string.IsNullOrEmpty(adminPasswordHash))
            {
                var adminPassword = _configuration["Admin:Password"];
                if (!string.IsNullOrEmpty(adminPassword))
                {
                    // Log warning - password should be hashed in production
                    _logger.LogWarning("Admin password is stored in plain text. Generate hash using HashPassword() method.");
                    adminPasswordHash = HashPassword(adminPassword);
                }
            }

            // Verify credentials
            if (!string.IsNullOrEmpty(adminEmail) && 
                !string.IsNullOrEmpty(adminPasswordHash) &&
                email.Equals(adminEmail, StringComparison.OrdinalIgnoreCase) &&
                VerifyPassword(password, adminPasswordHash))
            {
                // Create claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Sign in
                await context.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    });

                _logger.LogInformation($"Admin user {email} logged in successfully");
                context.Response.Redirect("/Admin/Dashboard");
            }
            else
            {
                _logger.LogWarning($"Failed login attempt for {email}");
                context.Response.Redirect("/Admin/Account/Login?error=InvalidCredentials");
            }
        }

        private async Task HandleLogoutAsync(HttpContext context)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("Admin user logged out");
            context.Response.Redirect("/");
        }

        /// <summary>
        /// Hash password using PBKDF2 with SHA256, 100,000 iterations, 16-byte salt
        /// Output format: base64(salt) + ":" + base64(hash)
        /// </summary>
        public static string HashPassword(string password)
        {
            // Generate random salt
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            // Hash password with PBKDF2
            using var pbkdf2 = new Rfc2898DeriveBytes(
                password: password,
                salt: salt,
                iterations: 100_000,
                hashAlgorithm: HashAlgorithmName.SHA256);

            byte[] hash = pbkdf2.GetBytes(32); // 256 bits

            // Combine salt and hash
            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Verify password against stored hash (timing-attack resistant)
        /// </summary>
        private static bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                var parts = storedHash.Split(':');
                if (parts.Length != 2) return false;

                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] storedHashBytes = Convert.FromBase64String(parts[1]);

                // Hash input password with stored salt
                using var pbkdf2 = new Rfc2898DeriveBytes(
                    password: password,
                    salt: salt,
                    iterations: 100_000,
                    hashAlgorithm: HashAlgorithmName.SHA256);

                byte[] computedHash = pbkdf2.GetBytes(32);

                // Timing-attack resistant comparison
                return CryptographicOperations.FixedTimeEquals(computedHash, storedHashBytes);
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Extension method to register SimpleAdminAuth in Program.cs
    /// </summary>
    public static class SimpleAdminAuthExtensions
    {
        public static IApplicationBuilder UseSimpleAdminAuth(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SimpleAdminAuthMiddleware>();
        }
    }

    /* ========================================
     * SETUP INSTRUCTIONS (Alternative to Identity)
     * ========================================
     * 
     * 1. REMOVE ASP.NET Core Identity from Program.cs:
     *    - Remove AddIdentity<IdentityUser, IdentityRole>()
     *    - Remove AddEntityFrameworkStores<ApplicationDbContext>()
     *    - Remove AdminSeeder.SeedAdminUserAsync()
     * 
     * 2. ADD Simple Auth in Program.cs:
     * 
     *    using MarutiTrainingPortal.Middleware;
     * 
     *    // Add cookie authentication
     *    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
     *        .AddCookie(options =>
     *        {
     *            options.LoginPath = "/Admin/Account/Login";
     *            options.LogoutPath = "/Admin/Account/Logout";
     *            options.AccessDeniedPath = "/Admin/Account/AccessDenied";
     *            options.ExpireTimeSpan = TimeSpan.FromHours(8);
     *            options.SlidingExpiration = true;
     *        });
     * 
     *    // After app.UseAuthentication();
     *    app.UseSimpleAdminAuth();
     * 
     * 3. GENERATE PASSWORD HASH:
     * 
     *    using MarutiTrainingPortal.Middleware;
     *    
     *    var hash = SimpleAdminAuthMiddleware.HashPassword("YourSecurePassword123!");
     *    Console.WriteLine(hash);
     *    // Output: xJ8k3L2m... (copy this)
     * 
     * 4. SET USER SECRETS:
     * 
     *    dotnet user-secrets set "Admin:Email" "admin@marutitraining.com"
     *    dotnet user-secrets set "Admin:PasswordHash" "xJ8k3L2m..." (paste hash from step 3)
     * 
     * 5. PRODUCTION (Azure/Docker):
     * 
     *    ASPNETCORE_Admin__Email=admin@marutitraining.com
     *    ASPNETCORE_Admin__PasswordHash=xJ8k3L2m... (paste hash)
     * 
     * 6. ROTATE PASSWORD:
     * 
     *    - Generate new hash: SimpleAdminAuthMiddleware.HashPassword("NewPassword456!")
     *    - Update user secrets / environment variables
     *    - Restart app (no database changes needed)
     * 
     * ========================================
     * LIMITATIONS (vs. ASP.NET Core Identity)
     * ========================================
     * 
     * ❌ No lockout protection (must implement manually or use rate limiting)
     * ❌ No email confirmation workflow
     * ❌ No password reset functionality
     * ❌ No two-factor authentication
     * ❌ No user/role database tables (credentials in config only)
     * ❌ No password history / expiration policies
     * ❌ No claims-based authorization beyond basic role check
     * 
     * ✅ WHEN TO USE THIS:
     * - Simple admin-only sites with 1-2 users
     * - Prototype/MVP projects
     * - When database schema must be minimal
     * - When you control credential rotation manually
     * 
     * ✅ WHEN TO USE IDENTITY INSTEAD:
     * - Multiple admin users
     * - Need lockout, password reset, email confirmation
     * - Future-proofing for user management features
     * - Compliance requirements (audit trails, password policies)
     * 
     */
}
