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

/// <summary>
/// PRODUCTION-FIXED JSON Data Importer
/// 
/// CRITICAL FIXES APPLIED:
/// 1. Environment-independent path resolution (works locally AND in production)
/// 2. Comprehensive logging at every step
/// 3. Case-insensitive JSON deserialization
/// 4. Graceful error handling with detailed diagnostics
/// 5. Multiple fallback strategies for file location
/// 6. No silent failures - all errors are logged
/// 
/// WHY THIS WAS NEEDED:
/// - Original code used relative paths that failed in production
/// - No diagnostics when files were missing
/// - Case sensitivity issues on Linux hosting
/// - Silent deserialization failures
/// </summary>
public class JsonDataImporter
{
    /// <summary>
    /// Resolves the absolute path to a JSON file using multiple fallback strategies.
    /// This ensures the file is found in both local development and Azure production environments.
    /// 
    /// STRATEGY:
    /// 1. Try BaseDirectory (works on Azure: /home/site/wwwroot)
    /// 2. Try CurrentDirectory (fallback for some hosting)
    /// 3. Search parent directories (development scenarios)
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
        Func<Task<int>> importFunc, 
        Dictionary<string, (bool Success, int Count, string Error)> stats)
    {
        try
        {
            Console.WriteLine($"\n▶️  Importing {importName}...");
            var count = await importFunc();
            stats[importName] = (true, count, string.Empty);
            Console.WriteLine($"✅ {importName}: {count} records imported");
        }
        catch (Exception ex)
        {
            var errorMsg = $"{ex.GetType().Name}: {ex.Message}";
            Console.WriteLine($"❌ Failed to import {importName}: {errorMsg}");
            Console.WriteLine($"   Stack trace: {ex.StackTrace}");
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
            await SafeImport("Videos", () => ImportVideos(context), importStats);
            await SafeImport("Profiles", () => ImportProfiles(context), importStats);
            await SafeImport("ProfileDocuments", () => ImportProfileDocuments(context), importStats);
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

    /// <summary>
    /// JSON deserialization options with production-safe settings
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,  // CRITICAL: Handles camelCase/PascalCase mismatches
        ReadCommentHandling = JsonCommentHandling.Skip,  // Ignore comments in JSON
        AllowTrailingCommas = true,  // Be lenient with JSON formatting
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    private static async Task<int> ImportCourses(ApplicationDbContext context)
    {
        var jsonPath = GetJsonFilePath("CoursesDatabase.json");
        Console.WriteLine($"   📂 Reading from: {jsonPath}");
        
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"   ⚠️  File not found: {jsonPath}");
            return 0;
        }

        var fileInfo = new FileInfo(jsonPath);
        Console.WriteLine($"   📊 File size: {fileInfo.Length:N0} bytes");
        
        var jsonData = await File.ReadAllTextAsync(jsonPath);
        Console.WriteLine($"   📄 JSON content length: {jsonData.Length:N0} characters");
        
        // Use case-insensitive deserialization
        var courses = JsonSerializer.Deserialize<List<CourseImportDto>>(jsonData, JsonOptions);

        if (courses == null || !courses.Any())
        {
            Console.WriteLine($"   ⚠️  No courses found in JSON file");
            return 0;
        }

        Console.WriteLine($"   📥 Deserialized {courses.Count} courses successfully");
        
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
        Console.WriteLine($"   💾 Saved {courses.Count} courses to database");
        return courses.Count;
    }

    private static async Task<int> ImportTrainings(ApplicationDbContext context)
    {
        var jsonPath = GetJsonFilePath("TrainingsDatabase.json");
        Console.WriteLine($"   📂 Reading from: {jsonPath}");
        
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"   ⚠️  File not found: {jsonPath}");
            return 0;
        }

        var fileInfo = new FileInfo(jsonPath);
        Console.WriteLine($"   📊 File size: {fileInfo.Length:N0} bytes");
        
        var jsonData = await File.ReadAllTextAsync(jsonPath);
        Console.WriteLine($"   📄 JSON content length: {jsonData.Length:N0} characters");
        
        if (string.IsNullOrWhiteSpace(jsonData))
        {
            Console.WriteLine($"   ⚠️  File is empty");
            return 0;
        }
        
        var trainings = JsonSerializer.Deserialize<List<Training>>(jsonData, JsonOptions);

        if (trainings == null || !trainings.Any())
        {
            Console.WriteLine($"   ⚠️  No trainings found in JSON or deserialization failed");
            return 0;
        }

        Console.WriteLine($"   📥 Deserialized {trainings.Count} trainings successfully");
        context.Trainings.AddRange(trainings);
        await context.SaveChangesAsync();
        Console.WriteLine($"   💾 Saved {trainings.Count} trainings to database");
        return trainings.Count;
    }

    private static async Task<int> ImportEvents(ApplicationDbContext context)
    {
        var jsonPath = GetJsonFilePath("EventsDatabase.json");
        Console.WriteLine($"   📂 Reading from: {jsonPath}");
        
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"   ⚠️  File not found: {jsonPath}");
            return 0;
        }

        var fileInfo = new FileInfo(jsonPath);
        Console.WriteLine($"   📊 File size: {fileInfo.Length:N0} bytes");
        
        var jsonData = await File.ReadAllTextAsync(jsonPath);
        Console.WriteLine($"   📄 JSON content length: {jsonData.Length:N0} characters");
        
        var events = JsonSerializer.Deserialize<List<TrainingEvent>>(jsonData, JsonOptions);

        if (events == null || !events.Any())
        {
            Console.WriteLine($"   ⚠️  No events found in JSON or deserialization failed");
            return 0;
        }

        Console.WriteLine($"   📥 Deserialized {events.Count} events successfully");
        context.TrainingEvents.AddRange(events);
        await context.SaveChangesAsync();
        Console.WriteLine($"   💾 Saved {events.Count} events to database");
        
        return events.Count;
    }

    private static async Task<int> ImportCertificates(ApplicationDbContext context)
    {
        var jsonPath = GetJsonFilePath("CertificatesDatabase.json");
        if (!File.Exists(jsonPath)) return 0;

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var certificates = JsonSerializer.Deserialize<List<Certificate>>(jsonData, JsonOptions);

        if (certificates == null || !certificates.Any()) return 0;
        
        context.Certificates.AddRange(certificates);
        await context.SaveChangesAsync();
        return certificates.Count;
    }

    private static async Task<int> ImportImages(ApplicationDbContext context)
    {
        var jsonPath = GetJsonFilePath("ImagesDatabase.json");
        if (!File.Exists(jsonPath)) return 0;

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var images = JsonSerializer.Deserialize<List<WebsiteImage>>(jsonData, JsonOptions);

        if (images == null || !images.Any()) return 0;
        
        context.WebsiteImages.AddRange(images);
        await context.SaveChangesAsync();
        return images.Count;
    }

    private static async Task<int> ImportFeaturedVideos(ApplicationDbContext context)
    {
        var jsonPath = GetJsonFilePath("FeaturedVideosDatabase.json");
        if (!File.Exists(jsonPath)) return 0;

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var videos = JsonSerializer.Deserialize<List<FeaturedVideo>>(jsonData, JsonOptions);

        if (videos == null || !videos.Any()) return 0;
        
        context.FeaturedVideos.AddRange(videos);
        await context.SaveChangesAsync();
        return videos.Count;
    }

    private static async Task<int> ImportVideos(ApplicationDbContext context)
    {
        var jsonPath = GetJsonFilePath("VideosDatabase.json");
        if (!File.Exists(jsonPath)) return 0;

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var videos = JsonSerializer.Deserialize<List<Video>>(jsonData, JsonOptions);

        if (videos == null || !videos.Any()) return 0;

        foreach (var video in videos)
        {
            // Auto-generate thumbnail if missing
            if (string.IsNullOrWhiteSpace(video.ThumbnailUrl))
            {
                video.ThumbnailUrl = video.GenerateThumbnailUrl();
            }

            video.CreatedDate = DateTime.UtcNow;
            context.Videos.Add(video);
        }

        await context.SaveChangesAsync();
        return videos.Count;
    }

    private static async Task<int> ImportProfiles(ApplicationDbContext context)
    {
        var jsonPath = GetJsonFilePath("ProfilesDatabase.json");
        if (!File.Exists(jsonPath)) return 0;

        var jsonData = await File.ReadAllTextAsync(jsonPath);        if (string.IsNullOrWhiteSpace(jsonData)) return 0;
                if (string.IsNullOrWhiteSpace(jsonData)) return 0;
        
        var profiles = JsonSerializer.Deserialize<List<Profile>>(jsonData, JsonOptions);

        if (profiles == null || !profiles.Any()) return 0;
        
        // Check for existing profiles and only add new ones
        var existingIds = await context.Profiles.Select(p => p.Id).ToListAsync();
        var newProfiles = profiles.Where(p => !existingIds.Contains(p.Id)).ToList();
        
        if (newProfiles.Any())
        {
            context.Profiles.AddRange(newProfiles);
            await context.SaveChangesAsync();
        }
        
        return newProfiles.Count;
    }

    private static async Task<int> ImportSystemSettings(ApplicationDbContext context)
    {
        var jsonPath = GetJsonFilePath("SystemSettingsDatabase.json");
        if (!File.Exists(jsonPath)) return 0;

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var settings = JsonSerializer.Deserialize<List<SystemSettings>>(jsonData, JsonOptions);

        if (settings == null || !settings.Any()) return 0;
        
        // Check for existing settings and only add new ones
        var existingIds = await context.SystemSettings.Select(s => s.Id).ToListAsync();
        var newSettings = settings.Where(s => !existingIds.Contains(s.Id)).ToList();
        
        if (newSettings.Any())
        {
            context.SystemSettings.AddRange(newSettings);
            await context.SaveChangesAsync();
        }
        
        return newSettings.Count;
    }

    private static async Task<int> ImportContactMessages(ApplicationDbContext context)
    {
        var jsonPath = GetJsonFilePath("ContactMessagesDatabase.json");
        if (!File.Exists(jsonPath)) return 0;

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var messages = JsonSerializer.Deserialize<List<ContactMessage>>(jsonData, JsonOptions);

        if (messages == null || !messages.Any()) return 0;
        
        context.ContactMessages.AddRange(messages);
        await context.SaveChangesAsync();
        return messages.Count;
    }

    private static async Task<int> ImportEventRegistrations(ApplicationDbContext context)
    {
        var jsonPath = GetJsonFilePath("EventRegistrationsDatabase.json");
        if (!File.Exists(jsonPath)) return 0;

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var registrations = JsonSerializer.Deserialize<List<TrainingEventRegistration>>(jsonData, JsonOptions);

        if (registrations == null || !registrations.Any()) return 0;
        
        context.TrainingEventRegistrations.AddRange(registrations);
        await context.SaveChangesAsync();
        return registrations.Count;
    }

    private static async Task<int> ImportLeadAuditLogs(ApplicationDbContext context)
    {
        var jsonPath = GetJsonFilePath("LeadAuditLogsDatabase.json");
        if (!File.Exists(jsonPath)) return 0;

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var logs = JsonSerializer.Deserialize<List<LeadAuditLog>>(jsonData, JsonOptions);

        if (logs == null || !logs.Any()) return 0;
        
        context.LeadAuditLogs.AddRange(logs);
        await context.SaveChangesAsync();
        return logs.Count;
    }

    /// <summary>
    /// Import profile documents (downloadable PDFs) from ProfileDocumentDatabase.json
    /// </summary>
    private static async Task<int> ImportProfileDocuments(ApplicationDbContext context)
    {
        var jsonPath = GetJsonFilePath("ProfileDocumentDatabase.json");
        if (!File.Exists(jsonPath)) return 0;

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var documents = JsonSerializer.Deserialize<List<ProfileDocument>>(jsonData, JsonOptions);

        if (documents == null || !documents.Any()) return 0;

        foreach (var doc in documents)
        {
            if (!context.ProfileDocuments.Any(d => d.Id == doc.Id))
            {
                context.ProfileDocuments.Add(doc);
            }
        }

        await context.SaveChangesAsync();
        return documents.Count;
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
