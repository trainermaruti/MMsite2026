using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;

public class CourseImporter
{
    public static async Task ImportCourses(ApplicationDbContext context)
    {
        var jsonData = await File.ReadAllTextAsync("JsonData/CoursesDatabase.json");
        var courses = JsonSerializer.Deserialize<List<CourseImportDto>>(jsonData);

        if (courses == null || !courses.Any())
        {
            Console.WriteLine("No courses found to import.");
            return;
        }

        foreach (var courseDto in courses)
        {
            var course = new Course
            {
                Title = courseDto.Title,
                Description = courseDto.Description,
                Category = courseDto.Category,
                Level = courseDto.Level,
                DurationMinutes = courseDto.DurationMinutes,
                DurationSeconds = 0,
                VideoUrl = courseDto.SkillTechUrl, // Store SkillTech URL instead of YouTube
                ThumbnailUrl = courseDto.ThumbnailUrl,
                Price = courseDto.Price,
                SkillTechUrl = courseDto.SkillTechUrl,
                Rating = 4.8,
                TotalEnrollments = 0,
                PublishedDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            context.Courses.Add(course);
        }

        await context.SaveChangesAsync();
        Console.WriteLine($"Successfully imported {courses.Count} courses!");
    }
}

public class CourseImportDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string VideoUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string SkillTechUrl { get; set; } = string.Empty;
}
