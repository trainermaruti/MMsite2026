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
    public static async Task ImportAllData(ApplicationDbContext context)
    {
        try
        {
            Console.WriteLine("🔄 Starting JSON data import...");
            
            // Import in order of dependencies
            await ImportCourses(context);
            await ImportTrainings(context);
            await ImportEvents(context);
            await ImportEventRegistrations(context);
            await ImportCertificates(context);
            await ImportImages(context);
            await ImportFeaturedVideos(context);
            await ImportProfiles(context);
            await ImportSystemSettings(context);
            await ImportContactMessages(context);
            await ImportLeadAuditLogs(context);
            
            Console.WriteLine("✅ JSON data import completed successfully!");
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
            Console.WriteLine("Courses already exist, skipping import.");
            return;
        }

        var jsonPath = Path.Combine("JsonData", "CoursesDatabase.json");
        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"⚠️ File not found: {jsonPath}");
            return;
        }

        var jsonData = await File.ReadAllTextAsync(jsonPath);
        var courses = JsonSerializer.Deserialize<List<CourseImportDto>>(jsonData);

        if (courses != null && courses.Any())
        {
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
            Console.WriteLine($"✓ Imported {courses.Count} courses");
        }
    }

    private static async Task ImportTrainings(ApplicationDbContext context)
    {
        if (await context.Trainings.AnyAsync())
        {
            Console.WriteLine("Trainings already exist, skipping import.");
            return;
        }

        var jsonPath = Path.Combine("JsonData", "TrainingsDatabase.json");
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

        var jsonPath = Path.Combine("JsonData", "EventsDatabase.json");
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

        var jsonPath = Path.Combine("JsonData", "CertificatesDatabase.json");
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

        var jsonPath = Path.Combine("JsonData", "ImagesDatabase.json");
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

        var jsonPath = Path.Combine("JsonData", "FeaturedVideosDatabase.json");
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

        var jsonPath = Path.Combine("JsonData", "ProfilesDatabase.json");
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

        var jsonPath = Path.Combine("JsonData", "SystemSettingsDatabase.json");
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

        var jsonPath = Path.Combine("JsonData", "ContactMessagesDatabase.json");
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

        var jsonPath = Path.Combine("JsonData", "EventRegistrationsDatabase.json");
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

        var jsonPath = Path.Combine("JsonData", "LeadAuditLogsDatabase.json");
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
