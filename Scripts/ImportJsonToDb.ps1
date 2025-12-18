# Script to import JSON data into database using EF Core
$ErrorActionPreference = "Stop"

Write-Host "Starting JSON to Database import..." -ForegroundColor Cyan

# Navigate to project directory
Set-Location "C:\Users\Skill\Desktop\MMsite2026"

# Create a temporary C# script file for EF Core migration
$migrationScript = @"
using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Import data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Import Courses
    var coursesJson = await File.ReadAllTextAsync("JsonData/CoursesDatabase.json");
    var courses = JsonSerializer.Deserialize<List<Course>>(coursesJson);
    if (courses != null && courses.Any())
    {
        // Clear existing courses
        dbContext.Courses.RemoveRange(dbContext.Courses);
        await dbContext.SaveChangesAsync();
        
        // Add new courses
        await dbContext.Courses.AddRangeAsync(courses);
        await dbContext.SaveChangesAsync();
        Console.WriteLine(`$"Imported {courses.Count} courses");
    }
    
    // Import Trainings
    var trainingsJson = await File.ReadAllTextAsync("JsonData/TrainingsDatabase.json");
    var trainings = JsonSerializer.Deserialize<List<Training>>(trainingsJson);
    if (trainings != null && trainings.Any())
    {
        dbContext.Trainings.RemoveRange(dbContext.Trainings);
        await dbContext.SaveChangesAsync();
        
        await dbContext.Trainings.AddRangeAsync(trainings);
        await dbContext.SaveChangesAsync();
        Console.WriteLine(`$"Imported {trainings.Count} trainings");
    }
    
    // Import Events
    var eventsJson = await File.ReadAllTextAsync("JsonData/EventsDatabase.json");
    var events = JsonSerializer.Deserialize<List<TrainingEvent>>(eventsJson);
    if (events != null && events.Any())
    {
        dbContext.TrainingEvents.RemoveRange(dbContext.TrainingEvents);
        await dbContext.SaveChangesAsync();
        
        await dbContext.TrainingEvents.AddRangeAsync(events);
        await dbContext.SaveChangesAsync();
        Console.WriteLine(`$"Imported {events.Count} events");
    }
}

Console.WriteLine("Import completed successfully!");
"@

# Save the script
$tempFile = "Scripts\TempImport.cs"
$migrationScript | Out-File -FilePath $tempFile -Encoding UTF8

Write-Host "Migration script created. Compiling..." -ForegroundColor Yellow

# Run the import using dotnet script
dotnet script $tempFile

Write-Host "Import complete!" -ForegroundColor Green
