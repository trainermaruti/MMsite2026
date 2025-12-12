// Run this file with: dotnet run --project MarutiTrainingPortal.csproj -- seed-images
using Microsoft.EntityFrameworkCore;
using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;

public class SeedInitialImages
{
        public static async Task SeedImages(ApplicationDbContext context)
        {
            // Check if images already exist
            if (await context.WebsiteImages.AnyAsync())
            {
                Console.WriteLine("Website images already seeded.");
                return;
            }

            var images = new List<WebsiteImage>
            {
                new WebsiteImage
                {
                    ImageKey = "profile_main",
                    DisplayName = "Main Profile Picture",
                    Description = "Profile picture displayed in header navigation",
                    ImageUrl = "/images/44.png",
                    AltText = "Profile Picture",
                    Category = "Profile",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                },
                new WebsiteImage
                {
                    ImageKey = "profile_hero",
                    DisplayName = "About Section Photo",
                    Description = "Large profile photo in about section",
                    ImageUrl = "/images/22.png",
                    AltText = "Maruti Makwana",
                    Category = "About",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                },
                new WebsiteImage
                {
                    ImageKey = "experience_badge",
                    DisplayName = "Experience Badge",
                    Description = "Badge showing years of experience",
                    ImageUrl = "/images/experience-badge.png",
                    AltText = "Experience Badge",
                    Category = "Badge",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                }
            };

            await context.WebsiteImages.AddRangeAsync(images);
            await context.SaveChangesAsync();
            Console.WriteLine($"Successfully seeded {images.Count} website images.");
        }
    }
