using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Services;
using MarutiTrainingPortal.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// --- FIX 1: Use In-Memory Database instead of SQL Server ---
// This allows the app to run without a real SQL server connection.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("MarutiTrainingPortalDb"));

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

// Add SkillTech Review Service (dynamic reviews from SkillTech.club)
builder.Services.AddScoped<MarutiTrainingPortal.Services.ISkillTechReviewService, MarutiTrainingPortal.Services.SkillTechReviewService>();

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
        
        // --- FIX 2: Disable SQL Migrations ---
        // Migrations fail on In-Memory databases, so we skip this step.
        if (app.Environment.IsProduction())
        {
            Console.WriteLine("🔄 Running database migrations (SKIPPED for In-Memory)...");
            // await dbContext.Database.MigrateAsync(); 
            Console.WriteLine("✓ Database migrations skipped");
        }
        
        await AdminSeeder.SeedAdminUserAsync(scope.ServiceProvider, app.Configuration);
        
        // ALWAYS import JSON data for In-Memory database (it's empty on every restart)
        Console.WriteLine("📦 In-Memory Database detected - importing all data from JSON files...");
        Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");
        Console.WriteLine($"Base Directory: {AppDomain.CurrentDomain.BaseDirectory}");
        
        var jsonPath1 = Path.Combine(Directory.GetCurrentDirectory(), "JsonData");
        var jsonPath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JsonData");
        Console.WriteLine($"JsonData path 1 (CurrentDir): {jsonPath1}");
        Console.WriteLine($"   Exists: {Directory.Exists(jsonPath1)}");
        Console.WriteLine($"JsonData path 2 (BaseDir): {jsonPath2}");
        Console.WriteLine($"   Exists: {Directory.Exists(jsonPath2)}");
        
        if (Directory.Exists(jsonPath1))
        {
            Console.WriteLine($"Files in CurrentDir/JsonData:");
            foreach (var file in Directory.GetFiles(jsonPath1, "*.json"))
            {
                Console.WriteLine($"   - {Path.GetFileName(file)}");
            }
        }
        
        if (Directory.Exists(jsonPath2))
        {
            Console.WriteLine($"Files in BaseDir/JsonData:");
            foreach (var file in Directory.GetFiles(jsonPath2, "*.json"))
            {
                Console.WriteLine($"   - {Path.GetFileName(file)}");
            }
        }
        
        try
        {
            await MarutiTrainingPortal.Helpers.JsonDataImporter.ImportAllData(dbContext);
            Console.WriteLine("✅ JSON data import completed successfully!");
        }
        catch (Exception importEx)
        {
            Console.WriteLine($"❌ JSON import FAILED!");
            Console.WriteLine($"Error Type: {importEx.GetType().Name}");
            Console.WriteLine($"Error Message: {importEx.Message}");
            Console.WriteLine($"Stack Trace: {importEx.StackTrace}");
            if (importEx.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {importEx.InnerException.Message}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠ Database initialization error: {ex.Message}");
        Console.WriteLine("Application will continue but database features may not work.");
    }
}

// Health check endpoint for Azure monitoring and diagnostics
app.MapGet("/health", async (ApplicationDbContext dbContext) =>
{
    try
    {
        var coursesCount = await dbContext.Courses.CountAsync();
        var eventsCount = await dbContext.TrainingEvents.CountAsync();
        var profilesCount = await dbContext.Profiles.CountAsync();
        var imagesCount = await dbContext.WebsiteImages.CountAsync();
        
        var hasData = coursesCount > 0 || profilesCount > 0;
        
        return Results.Ok(new 
        { 
            status = "healthy", 
            timestamp = DateTime.UtcNow,
            database = "in-memory",
            dataLoaded = hasData,
            counts = new 
            {
                courses = coursesCount,
                events = eventsCount,
                profiles = profilesCount,
                images = imagesCount
            },
            paths = new
            {
                currentDir = Directory.GetCurrentDirectory(),
                baseDir = AppDomain.CurrentDomain.BaseDirectory,
                jsonDataExists = Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JsonData"))
            },
            environment = app.Environment.EnvironmentName
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: 500
        );
    }
});

// PRODUCTION DIAGNOSTIC ENDPOINT
// Access via: /json-diagnostics
// Shows detailed JSON file loading status for troubleshooting
app.MapGet("/json-diagnostics", (IWebHostEnvironment env) =>
{
    var diagnostics = new Dictionary<string, object>();
    
    try
    {
        // Path information
        diagnostics["Environment"] = env.EnvironmentName;
        diagnostics["ContentRootPath"] = env.ContentRootPath;
        diagnostics["WebRootPath"] = env.WebRootPath;
        diagnostics["BaseDirectory"] = AppDomain.CurrentDomain.BaseDirectory;
        diagnostics["CurrentDirectory"] = Directory.GetCurrentDirectory();
        
        var jsonDataPath = Path.Combine(env.ContentRootPath, "JsonData");
        diagnostics["JsonDataPath"] = jsonDataPath;
        diagnostics["JsonDataExists"] = Directory.Exists(jsonDataPath);
        
        // List JSON files
        if (Directory.Exists(jsonDataPath))
        {
            var files = Directory.GetFiles(jsonDataPath, "*.json")
                .Select(f =>
                {
                    var info = new FileInfo(f);
                    return new
                    {
                        Name = Path.GetFileName(f),
                        Size = info.Length,
                        SizeFormatted = $"{info.Length:N0} bytes",
                        LastModified = info.LastWriteTimeUtc,
                        FullPath = f,
                        Readable = IsFileReadable(f)
                    };
                })
                .ToList();
            
            diagnostics["JsonFiles"] = files;
            diagnostics["JsonFileCount"] = files.Count;
        }
        else
        {
            diagnostics["Error"] = "JsonData folder does not exist!";
            diagnostics["ExpectedPath"] = jsonDataPath;
            diagnostics["Suggestion"] = "Ensure .csproj has <CopyToOutputDirectory>Always</CopyToOutputDirectory> for JsonData files";
        }
        
        // Check parent directories
        var parentSearch = new List<object>();
        var searchDir = env.ContentRootPath;
        for (int i = 0; i < 3; i++)
        {
            var parent = Directory.GetParent(searchDir);
            if (parent == null) break;
            
            var parentJsonPath = Path.Combine(parent.FullName, "JsonData");
            parentSearch.Add(new
            {
                Level = i + 1,
                Path = parentJsonPath,
                Exists = Directory.Exists(parentJsonPath)
            });
            
            searchDir = parent.FullName;
        }
        diagnostics["ParentDirectorySearch"] = parentSearch;
        
        return Results.Ok(diagnostics);
    }
    catch (Exception ex)
    {
        diagnostics["FatalError"] = ex.Message;
        diagnostics["StackTrace"] = ex.StackTrace;
        return Results.Ok(diagnostics);
    }
});

// Helper function to check if file is readable
static bool IsFileReadable(string filePath)
{
    try
    {
        using var fs = File.OpenRead(filePath);
        return true;
    }
    catch
    {
        return false;
    }
}

// ADMIN DIAGNOSTICS - Check if admin user exists and credentials are configured
app.MapGet("/admin/check-admin", async (UserManager<IdentityUser> userManager, IConfiguration config) =>
{
    var html = "<html><head><style>body{font-family:monospace;padding:20px;background:#000;color:#0f0;}h1{color:#0ff;}.error{color:#f00;}.success{color:#0f0;}.warning{color:#ff0;}</style></head><body>";
    html += "<h1>🔐 Admin User Diagnostics</h1>";
    
    try
    {
        var adminEmail = config["Admin:Email"];
        var hasPassword = !string.IsNullOrEmpty(config["Admin:Password"]);
        
        html += "<h3>Configuration:</h3>";
        html += $"<p>Admin Email from config: {adminEmail ?? "<span class='error'>NOT SET</span>"}</p>";
        html += $"<p>Admin Password configured: {(hasPassword ? "<span class='success'>YES</span>" : "<span class='error'>NO</span>")}</p>";
        
        if (string.IsNullOrEmpty(adminEmail))
        {
            html += "<p class='error'>❌ Admin:Email is not configured!</p>";
            html += "<p class='warning'>Default credentials: admin@marutitraining.com / Admin@123</p>";
        }
        else
        {
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            
            if (adminUser == null)
            {
                html += $"<p class='error'>❌ Admin user '{adminEmail}' does NOT exist in database</p>";
                html += "<p class='warning'>Admin seeding may have failed during startup</p>";
            }
            else
            {
                html += $"<p class='success'>✅ Admin user '{adminEmail}' exists</p>";
                html += $"<p>User ID: {adminUser.Id}</p>";
                html += $"<p>Email Confirmed: {adminUser.EmailConfirmed}</p>";
                
                var roles = await userManager.GetRolesAsync(adminUser);
                html += $"<p>Roles: {string.Join(", ", roles)}</p>";
                
                if (!roles.Contains("Admin"))
                {
                    html += "<p class='error'>⚠️  User does not have Admin role!</p>";
                }
            }
        }
        
        html += "<h3>All Users:</h3><ul>";
        var allUsers = userManager.Users.ToList();
        foreach (var user in allUsers)
        {
            var userRoles = await userManager.GetRolesAsync(user);
            html += $"<li>{user.Email} - Roles: {string.Join(", ", userRoles)}</li>";
        }
        html += "</ul>";
        
        html += "<p><a href='/Account/Login' style='color:#0ff;'>Go to Login Page</a></p>";
    }
    catch (Exception ex)
    {
        html += $"<p class='error'>Error: {ex.Message}</p>";
    }
    
    html += "</body></html>";
    return Results.Content(html, "text/html");
});

// MANUAL IMPORT ENDPOINT - Trigger data import manually to see errors
// Visit: https://marutimakwana.azurewebsites.net/admin/force-import-data
app.MapGet("/admin/force-import-data", async (ApplicationDbContext dbContext) =>
{
    var html = "<html><head><style>body{font-family:monospace;padding:20px;background:#000;color:#0f0;}h1{color:#0ff;}.error{color:#f00;}.success{color:#0f0;}.warning{color:#ff0;}</style></head><body>";
    html += "<h1>🔄 Manual JSON Data Import</h1>";
    
    try
    {
        html += $"<p>📍 Base directory: {AppDomain.CurrentDomain.BaseDirectory}</p>";
        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JsonData");
        html += $"<p>📂 JsonData path: {jsonPath}</p>";
        html += $"<p>📂 JsonData exists: {Directory.Exists(jsonPath)}</p>";
        
        if (Directory.Exists(jsonPath))
        {
            html += "<h3>Files in JsonData:</h3><ul>";
            foreach (var file in Directory.GetFiles(jsonPath, "*.json"))
            {
                var fi = new FileInfo(file);
                html += $"<li>{Path.GetFileName(file)} - {fi.Length:N0} bytes</li>";
            }
            html += "</ul>";
        }
        
        // Clear existing data
        html += "<h3>🗑️  Clearing existing data...</h3>";
        dbContext.Courses.RemoveRange(dbContext.Courses);
        dbContext.TrainingEvents.RemoveRange(dbContext.TrainingEvents);
        dbContext.Profiles.RemoveRange(dbContext.Profiles);
        dbContext.WebsiteImages.RemoveRange(dbContext.WebsiteImages);
        dbContext.Trainings.RemoveRange(dbContext.Trainings);
        await dbContext.SaveChangesAsync();
        html += "<p class='success'>✅ Cleared!</p>";
        
        html += "<h3>📥 Starting import...</h3>";
        try
        {
            await MarutiTrainingPortal.Helpers.JsonDataImporter.ImportAllData(dbContext);
            html += "<p class='success'>✅ Import completed!</p>";
        }
        catch (Exception importEx)
        {
            html += $"<p class='error'>❌ Import failed: {importEx.GetType().Name}</p>";
            html += $"<p class='error'>Error: {importEx.Message}</p>";
            html += $"<pre class='error'>{importEx.StackTrace}</pre>";
            if (importEx.InnerException != null)
            {
                html += $"<p class='error'>Inner: {importEx.InnerException.Message}</p>";
            }
        }
        
        // Check counts
        html += "<h3>📊 Database Counts:</h3><ul>";
        html += $"<li>Courses: {await dbContext.Courses.CountAsync()}</li>";
        html += $"<li>Events: {await dbContext.TrainingEvents.CountAsync()}</li>";
        html += $"<li>Trainings: {await dbContext.Trainings.CountAsync()}</li>";
        html += $"<li>Profiles: {await dbContext.Profiles.CountAsync()}</li>";
        html += $"<li>Images: {await dbContext.WebsiteImages.CountAsync()}</li>";
        html += "</ul>";
        
        html += "<p><a href='/health' style='color:#0ff;'>Check /health endpoint</a></p>";
        html += "<p><a href='/' style='color:#0ff;'>Go to homepage</a></p>";
    }
    catch (Exception ex)
    {
        html += $"<p class='error'>Fatal error: {ex.Message}</p>";
        html += $"<pre class='error'>{ex.StackTrace}</pre>";
    }
    
    html += "</body></html>";
    return Results.Content(html, "text/html");
});

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