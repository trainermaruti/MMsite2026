using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;

namespace MarutiTrainingPortal.Helpers;

public class JsonDataImporter
{
    /// <summary>
    /// Resolves the absolute path to a JSON file using multiple fallback strategies.
    /// This ensures the file is found in both local development and Azure production environments.
    /// </summary>
    /// <param name="fileName">The name of the JSON file (e.g., "CoursesDatabase.json")</param>
    /// <returns>Absolute path to the JSON file</returns>
    /// <exception cref="FileNotFoundException">Thrown when file cannot be found in any location</exception>
    private static string GetJsonFilePath(string fileName)
    {
        Console.WriteLine($"\n🔍 RESOLVING PATH FOR: {fileName}");
        Console.WriteLine("─".PadRight(80, '─'));
        
        // Strategy 1: AppDomain.CurrentDomain.BaseDirectory (Works on Azure)
        // Azure: C:\home\site\wwwroot\
        // Local: bin\Debug\net8.0\
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var path1 = Path.Combine(baseDir, "JsonData", fileName);
        Console.WriteLine($"Strategy 1 (BaseDirectory): {baseDir}");
        Console.WriteLine($"   → Full path: {path1}");
        Console.WriteLine($"   → Exists: {File.Exists(path1)}");
        
        if (File.Exists(path1))
        {
            Console.WriteLine("   ✅ FOUND via BaseDirectory");
            return path1;
        }
        
        // Strategy 2: Current Working Directory (Fallback for some hosting scenarios)
        var currentDir = Directory.GetCurrentDirectory();
        var path2 = Path.Combine(currentDir, "JsonData", fileName);
        Console.WriteLine($"\nStrategy 2 (CurrentDirectory): {currentDir}");
        Console.WriteLine($"   → Full path: {path2}");
        Console.WriteLine($"   → Exists: {File.Exists(path2)}");
        
        if (File.Exists(path2))
        {
            Console.WriteLine("   ✅ FOUND via CurrentDirectory");
            return path2;
        }
        
        // Strategy 3: Search parent directories (for development scenarios)
        var searchDir = baseDir;
        for (int i = 0; i < 5; i++) // Search up to 5 levels
        {
            var parentPath = Path.Combine(searchDir, "JsonData", fileName);
            if (File.Exists(parentPath))
            {
                Console.WriteLine($"\nStrategy 3 (Parent Search): Found at level {i}");
                Console.WriteLine($"   → Full path: {parentPath}");
                Console.WriteLine("   ✅ FOUND via Parent Directory Search");
                return parentPath;
            }
            
            var parent = Directory.GetParent(searchDir);
            if (parent == null) break;
            searchDir = parent.FullName;
        }
        
        // File not found - provide detailed diagnostics
        Console.WriteLine("\n❌ FILE NOT FOUND - DIAGNOSTICS:");
        Console.WriteLine($"   File Name: {fileName}");
        Console.WriteLine($"   BaseDirectory: {baseDir}");
        Console.WriteLine($"   CurrentDirectory: {currentDir}");
        
        // List what's actually in the JsonData folder (if it exists)
        var jsonDataDir1 = Path.Combine(baseDir, "JsonData");
        var jsonDataDir2 = Path.Combine(currentDir, "JsonData");
        
        if (Directory.Exists(jsonDataDir1))
        {
            Console.WriteLine($"\n   JsonData folder exists at: {jsonDataDir1}");
            Console.WriteLine("   Files found:");
            foreach (var file in Directory.GetFiles(jsonDataDir1))
            {
                Console.WriteLine($"      - {Path.GetFileName(file)}");
            }
        }
        else
        {
            Console.WriteLine($"   ⚠️  JsonData folder NOT found at: {jsonDataDir1}");
        }
        
        if (jsonDataDir1 != jsonDataDir2 && Directory.Exists(jsonDataDir2))
        {
            Console.WriteLine($"\n   JsonData folder exists at: {jsonDataDir2}");
            Console.WriteLine("   Files found:");
            foreach (var file in Directory.GetFiles(jsonDataDir2))
            {
                Console.WriteLine($"      - {Path.GetFileName(file)}");
            }
        }
        
        Console.WriteLine("\n   🔧 TROUBLESHOOTING STEPS:");
        Console.WriteLine("   1. Check if JsonData folder exists in deployment");
        Console.WriteLine("   2. Verify .csproj has <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>");
        Console.WriteLine("   3. Check Azure Kudu Console: site/wwwroot/JsonData/");
        Console.WriteLine("   4. Ensure file names match exactly (case-sensitive on Linux)");
        
        throw new FileNotFoundException(
            $"Could not find '{fileName}' in any of the expected locations. " +
            $"Checked: {path1}, {path2}, and parent directories. " +
            $"See console logs above for detailed diagnostics.");
    }

    private static async Task SafeImport(
        string importName, 
        Func<Task> importFunc, 
        Dictionary<string, (bool Success, int Count, string Error)> stats)
    {
        try
        {
            Console.WriteLine($"\n▶️  Importing {importName}...");
            await importFunc();
            // Success will be logged by individual import methods
        }
        catch (Exception ex)
        {
            var errorMsg = $"{ex.GetType().Name}: {ex.Message}";
            Console.WriteLine($"❌ Failed to import {importName}: {errorMsg}");
            stats[importName] = (false, 0, errorMsg);
        }
    }

    public static async Task ImportAllData(ApplicationDbContext context)
    {
        var startTime = DateTime.UtcNow;
        var importStats = new Dictionary<string, (bool Success, int Count, string Error)>();
        
        try
        {
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine("🔄 STARTING JSON DATA IMPORT");
            Console.WriteLine($"📍 Base directory: {AppDomain.CurrentDomain.BaseDirectory}");
            Console.WriteLine($"⏰ Start time: {startTime:yyyy-MM-dd HH:mm:ss} UTC");
            Console.WriteLine("=".PadRight(80, '='));
            
            // Import in order of dependencies with error tracking
            await SafeImport("Courses", () => ImportCourses(context), importStats);
            await SafeImport("Trainings", () => ImportTrainings(context), importStats);
            await SafeImport("Events", () => ImportEvents(context), importStats);
            await SafeImport("EventRegistrations", () => ImportEventRegistrations(context), importStats);
            await SafeImport("Certificates", () => ImportCertificates(context), importStats);
            await SafeImport("Images", () => ImportImages(context), importStats);
            await SafeImport("FeaturedVideos", () => ImportFeaturedVideos(context), importStats);
            await SafeImport("Profiles", () => ImportProfiles(context), importStats);
            await SafeImport("SystemSettings", () => ImportSystemSettings(context), importStats);
            await SafeImport("ContactMessages", () => ImportContactMessages(context), importStats);
            await SafeImport("LeadAuditLogs", () => ImportLeadAuditLogs(context), importStats);
            
            // Print summary
            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine("📊 IMPORT SUMMARY");
            Console.WriteLine($"⏱️  Total duration: {duration.TotalSeconds:F2} seconds");
            
            var successCount = importStats.Count(s => s.Value.Success);
            var failCount = importStats.Count(s => !s.Value.Success);
            var totalRecords = importStats.Sum(s => s.Value.Count);
            
            Console.WriteLine($"✅ Successful: {successCount}/{importStats.Count}");
            Console.WriteLine($"❌ Failed: {failCount}/{importStats.Count}");
            Console.WriteLine($"📦 Total records: {totalRecords}");
            
            if (failCount > 0)
            {
                Console.WriteLine("\n⚠️  FAILED IMPORTS:");
                foreach (var stat in importStats.Where(s => !s.Value.Success))
                {
                    Console.WriteLine($"   - {stat.Key}: {stat.Value.Error}");
                }
            }
            
            Console.WriteLine("=".PadRight(80, '='));
            
            if (successCount > 0)
            {
                Console.WriteLine("✅ JSON data import completed successfully!");
            }
            else
            {
                Console.WriteLine("⚠️  WARNING: No data was imported. Check file paths and permissions.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Error importing JSON data: {ex.Message}");
            throw;
        }
    }

    private static async Task ImportCourses(ApplicationDbContext context)
    {
        if (await context.Courses.AnyAsync())
        {
            Console.WriteLine("   ⏭️  Courses already exist, skipping import.");
            return;
        }

        var jsonPath = GetJsonFilePath("CoursesDatabase.json");
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"   ⚠️  File not found: {jsonPath}");
            throw new FileNotFoundException($"CoursesDatabase.json not found at {jsonPath}");
        }

        var fileInfo = new FileInfo(jsonPath);
        Console.WriteLine($"   📊 File size: {fileInfo.Length:N0} bytes");
        
        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var courses = JsonSerializer.Deserialize<List<CourseImportDto>>(jsonData);

        if (courses == null || !courses.Any())
        {
            Console.WriteLine($"   ⚠️  No courses found in JSON file");
            return;
        }

        Console.WriteLine($"   📥 Deserializ successful: {courses.Count} courses found");
        
        foreach (var dto in courses)
        {
            context.Courses.Add(new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                Level = dto.Level,
                DurationMinutes = dto.DurationMinutes,
                DurationSeconds = 0,
                VideoUrl = dto.SkillTechUrl,
                ThumbnailUrl = dto.ThumbnailUrl,
                Price = dto.Price,
                SkillTechUrl = dto.SkillTechUrl,
                Rating = 4.8,
                TotalEnrollments = 0,
                PublishedDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            });
        }
        
        await context.SaveChangesAsync();
        Console.WriteLine($"   ✅ Imported {courses.Count} courses");
    }

    private static async Task ImportTrainings(ApplicationDbContext context)
    {
        if (await context.Trainings.AnyAsync())
        {
            Console.WriteLine("Trainings already exist, skipping import.");
            return;
        }

        var jsonPath = GetJsonFilePath("TrainingsDatabase.json");
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"⚠️ File not found: {jsonPath}");
            return;
        }

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var trainings = JsonSerializer.Deserialize<List<Training>>(jsonData);

        if (trainings != null && trainings.Any())
        {
            context.Trainings.AddRange(trainings);
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ Imported {trainings.Count} trainings");
        }
    }

    private static async Task ImportEvents(ApplicationDbContext context)
    {
        if (await context.TrainingEvents.AnyAsync())
        {
            Console.WriteLine("Events already exist, skipping import.");
            return;
        }

        var jsonPath = GetJsonFilePath("EventsDatabase.json");
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"⚠️ File not found: {jsonPath}");
            return;
        }

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var events = JsonSerializer.Deserialize<List<TrainingEvent>>(jsonData);

        if (events != null && events.Any())
        {
            context.TrainingEvents.AddRange(events);
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ Imported {events.Count} events");
        }
    }

    private static async Task ImportCertificates(ApplicationDbContext context)
    {
        if (await context.Certificates.AnyAsync())
        {
            Console.WriteLine("Certificates already exist, skipping import.");
            return;
        }

        var jsonPath = GetJsonFilePath("CertificatesDatabase.json");
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"⚠️ File not found: {jsonPath}");
            return;
        }

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var certificates = JsonSerializer.Deserialize<List<Certificate>>(jsonData);

        if (certificates != null && certificates.Any())
        {
            context.Certificates.AddRange(certificates);
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ Imported {certificates.Count} certificates");
        }
    }

    private static async Task ImportImages(ApplicationDbContext context)
    {
        if (await context.WebsiteImages.AnyAsync())
        {
            Console.WriteLine("Images already exist, skipping import.");
            return;
        }

        var jsonPath = GetJsonFilePath("ImagesDatabase.json");
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"⚠️ File not found: {jsonPath}");
            return;
        }

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var images = JsonSerializer.Deserialize<List<WebsiteImage>>(jsonData);

        if (images != null && images.Any())
        {
            context.WebsiteImages.AddRange(images);
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ Imported {images.Count} images");
        }
    }

    private static async Task ImportFeaturedVideos(ApplicationDbContext context)
    {
        if (await context.FeaturedVideos.AnyAsync())
        {
            Console.WriteLine("Featured videos already exist, skipping import.");
            return;
        }

        var jsonPath = GetJsonFilePath("FeaturedVideosDatabase.json");
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"⚠️ File not found: {jsonPath}");
            return;
        }

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var videos = JsonSerializer.Deserialize<List<FeaturedVideo>>(jsonData);

        if (videos != null && videos.Any())
        {
            context.FeaturedVideos.AddRange(videos);
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ Imported {videos.Count} featured videos");
        }
    }

    private static async Task ImportProfiles(ApplicationDbContext context)
    {
        if (await context.Profiles.AnyAsync())
        {
            Console.WriteLine("Profiles already exist, skipping import.");
            return;
        }

        var jsonPath = GetJsonFilePath("ProfilesDatabase.json");
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"⚠️ File not found: {jsonPath}");
            return;
        }

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var profiles = JsonSerializer.Deserialize<List<Profile>>(jsonData);

        if (profiles != null && profiles.Any())
        {
            context.Profiles.AddRange(profiles);
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ Imported {profiles.Count} profiles");
        }
    }

    private static async Task ImportSystemSettings(ApplicationDbContext context)
    {
        if (await context.SystemSettings.AnyAsync())
        {
            Console.WriteLine("System settings already exist, skipping import.");
            return;
        }

        var jsonPath = GetJsonFilePath("SystemSettingsDatabase.json");
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"⚠️ File not found: {jsonPath}");
            return;
        }

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var settings = JsonSerializer.Deserialize<List<SystemSettings>>(jsonData);

        if (settings != null && settings.Any())
        {
            context.SystemSettings.AddRange(settings);
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ Imported {settings.Count} system settings");
        }
    }

    private static async Task ImportContactMessages(ApplicationDbContext context)
    {
        if (await context.ContactMessages.AnyAsync())
        {
            Console.WriteLine("Contact messages already exist, skipping import.");
            return;
        }

        var jsonPath = GetJsonFilePath("ContactMessagesDatabase.json");
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"⚠️ File not found: {jsonPath}");
            return;
        }

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var messages = JsonSerializer.Deserialize<List<ContactMessage>>(jsonData);

        if (messages != null && messages.Any())
        {
            context.ContactMessages.AddRange(messages);
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ Imported {messages.Count} contact messages");
        }
    }

    private static async Task ImportEventRegistrations(ApplicationDbContext context)
    {
        if (await context.TrainingEventRegistrations.AnyAsync())
        {
            Console.WriteLine("Event registrations already exist, skipping import.");
            return;
        }

        var jsonPath = GetJsonFilePath("EventRegistrationsDatabase.json");
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"⚠️ File not found: {jsonPath}");
            return;
        }

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var registrations = JsonSerializer.Deserialize<List<TrainingEventRegistration>>(jsonData);

        if (registrations != null && registrations.Any())
        {
            context.TrainingEventRegistrations.AddRange(registrations);
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ Imported {registrations.Count} event registrations");
        }
    }

    private static async Task ImportLeadAuditLogs(ApplicationDbContext context)
    {
        if (await context.LeadAuditLogs.AnyAsync())
        {
            Console.WriteLine("Lead audit logs already exist, skipping import.");
            return;
        }

        var jsonPath = GetJsonFilePath("LeadAuditLogsDatabase.json");
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"⚠️ File not found: {jsonPath}");
            return;
        }

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var logs = JsonSerializer.Deserialize<List<LeadAuditLog>>(jsonData);

        if (logs != null && logs.Any())
        {
            context.LeadAuditLogs.AddRange(logs);
            await context.SaveChangesAsync();
            Console.WriteLine($"✓ Imported {logs.Count} lead audit logs");
        }
    }
}

// DTO for course import
public class CourseImportDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string ThumbnailUrl { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string SkillTechUrl { get; set; } = string.Empty;
}
