using MarutiTrainingPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MarutiTrainingPortal.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Training> Trainings { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<TrainingEvent> TrainingEvents { get; set; }
        public DbSet<TrainingEventRegistration> TrainingEventRegistrations { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<LeadAuditLog> LeadAuditLogs { get; set; }
        public DbSet<SystemSettings> SystemSettings { get; set; }
        public DbSet<FeaturedVideo> FeaturedVideos { get; set; }
        public DbSet<WebsiteImage> WebsiteImages { get; set; }
        public DbSet<ProfileDocument> ProfileDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for Course Price
            modelBuilder.Entity<Course>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18,2)");

            // Add indexes for performance
            modelBuilder.Entity<Training>()
                .HasIndex(t => t.DeliveryDate);

            modelBuilder.Entity<Training>()
                .HasIndex(t => t.Title);

            modelBuilder.Entity<Course>()
                .HasIndex(c => c.Category);

            modelBuilder.Entity<Course>()
                .HasIndex(c => c.PublishedDate);

            modelBuilder.Entity<Course>()
                .HasIndex(c => c.Level);

            modelBuilder.Entity<TrainingEvent>()
                .HasIndex(e => e.StartDate);

            modelBuilder.Entity<ContactMessage>()
                .HasIndex(cm => cm.CreatedDate);

            modelBuilder.Entity<ContactMessage>()
                .HasIndex(cm => cm.Status);

            modelBuilder.Entity<ContactMessage>()
                .HasIndex(cm => cm.EventId);

            // Certificate indexes for fast lookup
            modelBuilder.Entity<Certificate>()
                .HasIndex(c => c.CertificateId)
                .IsUnique();

            modelBuilder.Entity<Certificate>()
                .HasIndex(c => c.StudentEmail);

            modelBuilder.Entity<Certificate>()
                .HasIndex(c => c.IsDeleted);

            // Soft delete query filter - exclude deleted records by default
            modelBuilder.Entity<Training>()
                .HasQueryFilter(t => !t.IsDeleted);

            modelBuilder.Entity<Course>()
                .HasQueryFilter(c => !c.IsDeleted);

            modelBuilder.Entity<TrainingEvent>()
                .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<ContactMessage>()
                .HasQueryFilter(cm => !cm.IsDeleted);

            modelBuilder.Entity<Certificate>()
                .HasQueryFilter(cert => !cert.IsDeleted);

            modelBuilder.Entity<FeaturedVideo>()
                .HasQueryFilter(fv => !fv.IsDeleted);

            modelBuilder.Entity<WebsiteImage>()
                .HasQueryFilter(wi => !wi.IsDeleted);

            // Configure relationships
            modelBuilder.Entity<ContactMessage>()
                .HasOne(cm => cm.Event)
                .WithMany()
                .HasForeignKey(cm => cm.EventId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<LeadAuditLog>()
                .HasOne(log => log.ContactMessage)
                .WithMany()
                .HasForeignKey(log => log.ContactMessageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed initial profile data
            modelBuilder.Entity<Profile>().HasData(
                new Profile
                {
                    Id = 1,
                    FullName = "Maruti Makwana",
                    Title = "Corporate Trainer - Azure Cloud & AI",
                    Bio = "Passionate corporate trainer with expertise in Azure Cloud Services and AI technologies. Dedicated to empowering IT professionals with cutting-edge skills.",
                    ProfileImageUrl = "/images/41258b2e-84fd-413e-a335-009a905a8742_44.png",
                    Email = "maruti_makwana@hotmail.com",
                    PhoneNumber = "+91 9998114148",
                    WhatsAppNumber = "+91 9081908127",
                    Expertise = "Azure Cloud, AI, Machine Learning, DevOps, Cloud Architecture",
                    TotalTrainingsDone = 50,
                    TotalStudents = 130000,
                    LinkedInUrl = "https://www.linkedin.com/in/marutimakwana/",
                    InstagramUrl = "https://www.instagram.com/marutimakwana?igsh=MWttazg1dGRkbTU3cg==",
                    YouTubeUrl = "https://www.youtube.com/@skilltechclub",
                    SkillTechUrl = "https://skilltech.club",
                    TwitterUrl = "https://twitter.com/maruti",
                    GitHubUrl = "https://github.com/maruti",
                    CertificationsAndAchievements = "Azure Certified Trainer, AI Specialist, AWS Certified Solutions Architect",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                }
            );

            // Seed System Settings
            modelBuilder.Entity<SystemSettings>().HasData(
                new SystemSettings
                {
                    Id = 1,
                    SiteTitle = "Maruti Makwana Training Portal",
                    MetaDescription = "Professional Azure & AI Training by Maruti Makwana - Corporate Trainer specializing in Cloud Computing and Artificial Intelligence",
                    MetaKeywords = "Azure Training, AI Training, Cloud Computing, Corporate Training, Machine Learning, DevOps",
                    EmailNotificationsEnabled = true,
                    MaintenanceMode = false,
                    ShowUpcomingEvents = true,
                    ShowCoursesSection = true,
                    ShowProfileSection = true,
                    AppVersion = "1.0.0",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                }
            );

            // Seed Admin Role
            string ADMIN_ROLE_ID = "2c5e174e-3b0e-446f-86af-483d56fd7210";
            string ADMIN_USER_ID = "8e445865-a24d-4543-a6c6-9443d048cdb9";

            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = ADMIN_ROLE_ID,
                Name = "Admin",
                NormalizedName = "ADMIN"
            });

            // Seed Admin User (Password: Meet@maruti1028)
            var hasher = new PasswordHasher<IdentityUser>();
            modelBuilder.Entity<IdentityUser>().HasData(new IdentityUser
            {
                Id = ADMIN_USER_ID,
                UserName = "maruti_makwana@hotmail.com",
                NormalizedUserName = "MARUTI_MAKWANA@HOTMAIL.COM",
                Email = "maruti_makwana@hotmail.com",
                NormalizedEmail = "MARUTI_MAKWANA@HOTMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Meet@maruti1028"),
                SecurityStamp = Guid.NewGuid().ToString()
            });

            // Assign Admin role to Admin user
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = ADMIN_ROLE_ID,
                UserId = ADMIN_USER_ID
            });

            // Seed Website Images
            modelBuilder.Entity<WebsiteImage>().HasData(
                new WebsiteImage
                {
                    Id = 1,
                    ImageKey = "profile_main",
                    DisplayName = "Profile Main Photo (DP)",
                    Description = "Main profile photo displayed in admin panel and website header",
                    ImageUrl = "/images/41258b2e-84fd-413e-a335-009a905a8742_44.png",
                    AltText = "Maruti Makwana Profile Photo",
                    Category = "Profile",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    IsDeleted = false
                },
                new WebsiteImage
                {
                    Id = 2,
                    ImageKey = "profile_hero",
                    DisplayName = "Profile Hero Image",
                    Description = "Large hero image on homepage profile section",
                    ImageUrl = "/images/22.png",
                    AltText = "Maruti Makwana Hero Image",
                    Category = "Profile",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    IsDeleted = false
                },
                new WebsiteImage
                {
                    Id = 3,
                    ImageKey = "experience_badge",
                    DisplayName = "Experience Badge",
                    Description = "Years of experience badge icon",
                    ImageUrl = "/images/experience-badge.png",
                    AltText = "Experience Badge",
                    Category = "Icons",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    IsDeleted = false
                }
            );
        }
    }
}
