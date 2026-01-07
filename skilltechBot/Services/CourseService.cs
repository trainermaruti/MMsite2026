using System.Text.Json;
using SkillTechNavigator.Models;

namespace SkillTechNavigator.Services
{
    public interface ICourseService
    {
        CourseCatalog GetCatalog();
        Course? GetCourse(string courseCode);
        List<Course> GetCoursesByLevel(string level);
        List<Course> GetFreeCourses();
        List<Course> GetPremiumCourses();
        LearningPath? GetLearningPath(string pathId);
        List<Course> GetCoursesForRole(string role);
        string GetCatalogContext();
    }

    public class CourseService : ICourseService
    {
        private readonly CourseCatalog _catalog;
        private readonly ILogger<CourseService> _logger;

        public CourseService(IWebHostEnvironment env, ILogger<CourseService> logger)
        {
            _logger = logger;
            var catalogPath = Path.Combine(env.WebRootPath, "data", "full_course_catalog.json");
            
            try
            {
                var jsonContent = File.ReadAllText(catalogPath);
                _catalog = JsonSerializer.Deserialize<CourseCatalog>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new CourseCatalog();
                
                _logger.LogInformation($"Course catalog loaded successfully. Total courses: {_catalog.Courses.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load course catalog. Using empty catalog.");
                _catalog = new CourseCatalog();
            }
        }

        public CourseCatalog GetCatalog() => _catalog;

        public Course? GetCourse(string courseCode)
        {
            return _catalog.Courses.FirstOrDefault(c => 
                c.Code.Equals(courseCode, StringComparison.OrdinalIgnoreCase) ||
                c.Id.Equals(courseCode, StringComparison.OrdinalIgnoreCase));
        }

        public List<Course> GetCoursesByLevel(string level)
        {
            return _catalog.Courses
                .Where(c => c.Level.Equals(level, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<Course> GetFreeCourses()
        {
            return _catalog.Courses.Where(c => c.IsFree).ToList();
        }

        public List<Course> GetPremiumCourses()
        {
            return _catalog.Courses.Where(c => c.IsPremium).ToList();
        }

        public LearningPath? GetLearningPath(string pathId)
        {
            return _catalog.LearningPaths.FirstOrDefault(p => 
                p.Id.Equals(pathId, StringComparison.OrdinalIgnoreCase));
        }

        public List<Course> GetCoursesForRole(string role)
        {
            return _catalog.Courses
                .Where(c => c.TargetRoles.Any(r => r.Contains(role, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        public string GetCatalogContext()
        {
            var context = "SKILLTECH COURSE CATALOG:\n\n";
            
            // Free Fundamentals
            context += "FREE FUNDAMENTALS:\n";
            foreach (var course in GetFreeCourses())
            {
                context += $"- {course.Code}: {course.Name} ({course.Duration})\n";
                context += $"  Prerequisites: {(course.Prerequisites.Any() ? string.Join(", ", course.Prerequisites) : "None")}\n";
                context += $"  Target: {string.Join(", ", course.TargetRoles)}\n";
                context += $"  URL: {course.Url}\n\n";
            }

            // Premium Courses
            context += "\nPREMIUM COURSES:\n";
            foreach (var course in GetPremiumCourses())
            {
                context += $"- {course.Code}: {course.Name} ({course.Duration})\n";
                context += $"  Level: {course.Level}\n";
                context += $"  Prerequisites: {string.Join(", ", course.Prerequisites)}\n";
                if (course.RequiresPrerequisiteCertification)
                {
                    context += $"  Required Certifications: {string.Join(", ", course.PrerequisiteCertifications)}\n";
                }
                context += $"  Target: {string.Join(", ", course.TargetRoles)}\n";
                context += $"  URL: {course.Url}\n\n";
            }

            // Learning Paths
            context += "\nLEARNING PATHS:\n";
            foreach (var path in _catalog.LearningPaths)
            {
                context += $"- {path.Name}: {path.Description}\n";
                context += $"  Courses: {string.Join(" → ", path.Courses.Select(id => GetCourse(id)?.Code ?? id))}\n";
                context += $"  Duration: {path.EstimatedDuration}\n";
                context += $"  For: {path.TargetAudience}\n\n";
            }

            // Products
            context += "\nPRODUCTS:\n";
            foreach (var product in _catalog.Products)
            {
                context += $"- {product.Name}: {product.Description}\n";
                context += $"  Price: {product.Price}\n";
                context += $"  URL: {product.Url}\n\n";
            }

            return context;
        }
    }
}
