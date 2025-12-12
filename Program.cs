using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Services;
using MarutiTrainingPortal.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext - SQLite for development, switch to SQL Server for production
// Production: use builder.Configuration.GetConnectionString("DefaultConnection")
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=MarutiTrainingPortal.db"));

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

// Add Email Service
builder.Services.AddScoped<IEmailSender, EmailSender>();

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

// Add Message Cleanup Service (28-day auto-delete)
builder.Services.AddScoped<IMessageCleanupService, MessageCleanupService>();

// Add Rate Limit Cleanup Background Service
builder.Services.AddHostedService<MarutiTrainingPortal.Middleware.RateLimitCleanupService>();

// Add Message Cleanup Background Service (auto-delete messages older than 28 days)
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
    await AdminSeeder.SeedAdminUserAsync(scope.ServiceProvider, app.Configuration);
    
    // Seed initial website images if running with seed-images argument
    if (args.Contains("seed-images"))
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedInitialImages.SeedImages(dbContext);
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

// Map Admin area routes
app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
