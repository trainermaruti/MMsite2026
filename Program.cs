using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Services;
using MarutiTrainingPortal.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext - SQL Server for all environments
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        Console.WriteLine("⚠ WARNING: DefaultConnection string is missing! Check Azure Configuration.");
        // Provide a fallback to prevent startup failure
        connectionString = "Server=localhost;Database=MarutiTrainingPortal;Integrated Security=true;TrustServerCertificate=True";
    }
    options.UseSqlServer(connectionString);
});

// Add Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure cookie authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Add HttpClient
builder.Services.AddHttpClient();

// Add CourseImportService
builder.Services.AddScoped<CourseImportService>();

// Add Email Service (using built-in EmailSender with updated interface)
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Add Contact Service
builder.Services.AddScoped<ContactService>();

// Add HTML Sanitizer Service (Ganss.XSS - free)
builder.Services.AddScoped<IHtmlSanitizerService, HtmlSanitizerService>();

// Add HtmlSanitizer directly for controllers that use it
builder.Services.AddScoped<Ganss.Xss.HtmlSanitizer>();

// Add Rate Limiting Service (in-memory for development)
builder.Services.AddSingleton<IRateLimitService, RateLimitService>();

// Add Memory Cache for statistics
builder.Services.AddMemoryCache();

// Add StatService for cached statistics
builder.Services.AddScoped<StatService>();

// Add Profile, Settings, and ImageUpload Services
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<IImageUploadService, ImageUploadService>();
builder.Services.AddScoped<IImageService, ImageService>();

// Add Chatbot Service (free LocalMock by default, optional OpenAI)
builder.Services.AddScoped<ChatbotService>();

// Add Contact Message Repository and Service
builder.Services.AddScoped<MarutiTrainingPortal.Repositories.IContactMessageRepository, MarutiTrainingPortal.Repositories.ContactMessageRepository>();
builder.Services.AddScoped<MarutiTrainingPortal.Services.IContactMessageService, MarutiTrainingPortal.Services.ContactMessageService>();

// Add Background Services
builder.Services.AddScoped<MarutiTrainingPortal.Services.IMessageCleanupService, MarutiTrainingPortal.Services.MessageCleanupService>();
builder.Services.AddHostedService<MarutiTrainingPortal.Middleware.RateLimitCleanupService>();
builder.Services.AddHostedService<MarutiTrainingPortal.Services.ContactMessageCleanupService>();
builder.Services.AddHostedService<MarutiTrainingPortal.Services.MessageCleanupBackgroundService>();

// Add session for rate limiting
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add antiforgery
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

var app = builder.Build();

// Seed admin user from configuration (User Secrets in dev, Environment Variables in prod)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Apply migrations automatically (for production deployment)
        if (app.Environment.IsProduction())
        {
            Console.WriteLine("🔄 Running database migrations...");
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("✓ Database migrations completed");
        }
        
        await AdminSeeder.SeedAdminUserAsync(scope.ServiceProvider, app.Configuration);
        
        // Import all data from JSON files if database is empty
        if (!await dbContext.Courses.AnyAsync())
        {
            Console.WriteLine("📦 Database is empty, importing data from JSON files...");
            await MarutiTrainingPortal.Helpers.JsonDataImporter.ImportAllData(dbContext);
        }
        else
        {
            Console.WriteLine("✓ Database already contains data, skipping JSON import");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠ Database initialization error: {ex.Message}");
        Console.WriteLine("Application will continue but database features may not work.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add Rate Limiting Middleware (before authentication)
app.UseMiddleware<MarutiTrainingPortal.Middleware.RateLimitMiddleware>();

// CRITICAL: Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// Ensure admin credentials are correct on startup (only if database is available)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Check if database is accessible
        if (await context.Database.CanConnectAsync())
        {
            var hasher = new PasswordHasher<IdentityUser>();
            
            // Check if the new admin already exists
            var newAdmin = context.Users.FirstOrDefault(u => u.NormalizedUserName == "MARUTI_MAKWANA@HOTMAIL.COM");
            
            if (newAdmin != null)
            {
                // Just update password and unlock account
                newAdmin.PasswordHash = hasher.HashPassword(newAdmin, "Meet@maruti1028");
                newAdmin.SecurityStamp = Guid.NewGuid().ToString();
                newAdmin.LockoutEnabled = false;
                newAdmin.AccessFailedCount = 0;
                newAdmin.LockoutEnd = null;
                await context.SaveChangesAsync();
                Console.WriteLine($"✓ Admin credentials verified: {newAdmin.Email}");
            }
        }
        else
        {
            Console.WriteLine("⚠ Database not accessible, skipping admin verification");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠ Admin verification error: {ex.Message}");
        Console.WriteLine("Application will continue without admin verification.");
    }
}

// Map Admin area routes
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();