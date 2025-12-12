using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarutiTrainingPortal.Models;
using MarutiTrainingPortal.Data;
using Microsoft.EntityFrameworkCore;

namespace MarutiTrainingPortal.Services
{
    /// <summary>
    /// Service to import courses from SkillTech Club website
    /// </summary>
    public class CourseImportService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public CourseImportService(ApplicationDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Imports predefined courses from SkillTech Club
        /// </summary>
        public async Task<List<Course>> ImportSkillTechCoursesAsync()
        {
            var courses = new List<Course>();

            try
            {
                // Get all courses from SkillTech Club
                var skillTechCourses = GetAvailableSkillTechCourses();

                foreach (var courseData in skillTechCourses)
                {
                    // Check if course already exists
                    var existingCourse = await _context.Courses
                        .FirstOrDefaultAsync(c => c.Title == courseData["Title"]);

                    if (existingCourse == null)
                    {
                        var course = new Course
                        {
                            Title = courseData["Title"],
                            Description = courseData["Description"],
                            Category = courseData["Category"],
                            Level = courseData["Level"],
                            ThumbnailUrl = courseData["ThumbnailUrl"],
                            VideoUrl = courseData["VideoUrl"],
                            DurationMinutes = int.Parse(courseData["DurationMinutes"]),
                            Price = string.IsNullOrEmpty(courseData["Price"]) ? 0 : decimal.Parse(courseData["Price"].Replace("₹", "").Replace("Free", "0")),
                            TotalEnrollments = int.Parse(courseData["Enrollments"]),
                            Rating = double.Parse(courseData["Rating"]),
                            PublishedDate = DateTime.Now
                        };

                        _context.Courses.Add(course);
                        courses.Add(course);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error importing courses: {ex.Message}", ex);
            }

            return courses;
        }

        /// <summary>
        /// Gets predefined SkillTech Club course data
        /// </summary>
        public List<Dictionary<string, string>> GetAvailableSkillTechCourses()
        {
            return new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    {"Title", "Azure Fundamentals Certification (AZ-900)"},
                    {"Description", "Master the basics of Microsoft Azure cloud platform. Learn cloud concepts, Azure services, pricing, and support. Perfect for beginners starting their cloud journey."},
                    {"Category", "Azure Fundamentals"},
                    {"Level", "Beginner"},
                    {"ThumbnailUrl", "https://stvidstrg25.blob.core.windows.net/skilltechimages/azure-fundamentals.webp"},
                    {"VideoUrl", "https://www.youtube.com/embed/NKEFWGpHUhk"},
                    {"SkillTechUrl", "https://skilltech.club/Courses/azure-fundamentals-certification/az-900-certification/1"},
                    {"DurationMinutes", "295"},
                    {"Price", "Free"},
                    {"Enrollments", "0"},
                    {"Rating", "4.8"}
                },
                new Dictionary<string, string>
                {
                    {"Title", "Azure AI Fundamentals Certification (AI-900)"},
                    {"Description", "Understand AI and machine learning concepts on Azure. Explore AI services, responsible AI principles, and hands-on demos. Ideal for those starting in AI."},
                    {"Category", "Azure AI"},
                    {"Level", "Beginner"},
                    {"ThumbnailUrl", "https://stvidstrg25.blob.core.windows.net/skilltechimages/azure-ai-900.webp"},
                    {"VideoUrl", "https://www.youtube.com/embed/UpKRw-6JmCU"},
                    {"SkillTechUrl", "https://skilltech.club/Courses/azure-ai-fundamentals-certification/ai-900-certification/2"},
                    {"DurationMinutes", "488"},
                    {"Price", "Free"},
                    {"Enrollments", "0"},
                    {"Rating", "4.9"}
                },
                new Dictionary<string, string>
                {
                    {"Title", "Azure Data Fundamentals Certification (DP-900)"},
                    {"Description", "Learn data fundamentals, databases, data warehousing, and analytics on Azure. Covers relational, non-relational databases, and modern data solutions."},
                    {"Category", "Azure Fundamentals"},
                    {"Level", "Beginner"},
                    {"ThumbnailUrl", "https://stvidstrg25.blob.core.windows.net/skilltechimages/azure-data-900.webp"},
                    {"VideoUrl", "https://www.youtube.com/embed/N0rZM9Sgg_w"},
                    {"SkillTechUrl", "https://skilltech.club/Courses/azure-data-fundamentals-certification/dp-900-certification/3"},
                    {"DurationMinutes", "218"},
                    {"Price", "Free"},
                    {"Enrollments", "0"},
                    {"Rating", "4.7"}
                },
                new Dictionary<string, string>
                {
                    {"Title", "Microsoft Copilots Studio Certification (AI-3018)"},
                    {"Description", "Build generative AI agents using Microsoft Copilot Studio. Create custom copilots, integrate with Microsoft services, and deploy intelligent solutions."},
                    {"Category", "Microsoft Copilot"},
                    {"Level", "Beginner"},
                    {"ThumbnailUrl", "https://stvidstrg25.blob.core.windows.net/skilltechimages/copilot-studio.webp"},
                    {"VideoUrl", "https://www.youtube.com/embed/nVJLr3-5OA4"},
                    {"SkillTechUrl", "https://skilltech.club/Courses/microsoft-copilot-studio/ai-3018-certification/4"},
                    {"DurationMinutes", "160"},
                    {"Price", "Free"},
                    {"Enrollments", "0"},
                    {"Rating", "4.8"}
                },
                new Dictionary<string, string>
                {
                    {"Title", "Azure Architect Expert Certification (AZ-305)"},
                    {"Description", "Design and architect Azure solutions at enterprise scale. Learn infrastructure design patterns, security, scalability, and governance best practices."},
                    {"Category", "Azure Architect"},
                    {"Level", "Advanced"},
                    {"ThumbnailUrl", "https://stvidstrg25.blob.core.windows.net/skilltechimages/azure-architect-305.webp"},
                    {"VideoUrl", "https://www.youtube.com/embed/xBl5v3VhQEk"},
                    {"SkillTechUrl", "https://skilltech.club/Courses/azure-solutions-architect/az-305-certification/5"},
                    {"DurationMinutes", "341"},
                    {"Price", "₹5999"},
                    {"Enrollments", "0"},
                    {"Rating", "4.9"}
                },
                new Dictionary<string, string>
                {
                    {"Title", "Azure Developer Certification (AZ-204)"},
                    {"Description", "Develop applications and services on Azure. Learn to build cloud-native solutions using Azure SDKs, APIs, storage, and compute services."},
                    {"Category", "Azure Developer"},
                    {"Level", "Intermediate"},
                    {"ThumbnailUrl", "https://stvidstrg25.blob.core.windows.net/skilltechimages/azure-developer-204.webp"},
                    {"VideoUrl", "https://www.youtube.com/embed/HrMc_otnWBs"},
                    {"SkillTechUrl", "https://skilltech.club/Courses/azure-developer/az-204-certification/6"},
                    {"DurationMinutes", "897"},
                    {"Price", "₹4999"},
                    {"Enrollments", "0"},
                    {"Rating", "4.8"}
                },
                new Dictionary<string, string>
                {
                    {"Title", "Azure Administrator Certification (AZ-104)"},
                    {"Description", "Manage Azure resources, subscriptions, identities, and governance. Learn infrastructure management, security, monitoring, and cost optimization."},
                    {"Category", "Azure Administrator"},
                    {"Level", "Intermediate"},
                    {"ThumbnailUrl", "https://stvidstrg25.blob.core.windows.net/skilltechimages/azure-admin-104.webp"},
                    {"VideoUrl", "https://www.youtube.com/embed/g9oCCnzn1wE"},
                    {"SkillTechUrl", "https://skilltech.club/Courses/azure-administrator/az-104-certification/7"},
                    {"DurationMinutes", "385"},
                    {"Price", "₹3999"},
                    {"Enrollments", "0"},
                    {"Rating", "4.7"}
                },
                new Dictionary<string, string>
                {
                    {"Title", "Azure AI Certification (AI-102)"},
                    {"Description", "Build intelligent applications with Azure AI services. Master computer vision, natural language processing, knowledge mining, and conversational AI."},
                    {"Category", "Azure AI"},
                    {"Level", "Intermediate"},
                    {"ThumbnailUrl", "https://stvidstrg25.blob.core.windows.net/skilltechimages/azure-ai-102.webp"},
                    {"VideoUrl", "https://www.youtube.com/embed/I7fdWafTcPY"},
                    {"SkillTechUrl", "https://skilltech.club/Courses/azure-ai-engineer/ai-102-certification/8"},
                    {"DurationMinutes", "427"},
                    {"Price", "₹6999"},
                    {"Enrollments", "0"},
                    {"Rating", "4.9"}
                },
                new Dictionary<string, string>
                {
                    {"Title", "Azure DevOps Engineer Certification (AZ-400)"},
                    {"Description", "Master CI/CD pipelines, DevOps practices, and cloud infrastructure automation. Build enterprise-grade deployment solutions on Azure."},
                    {"Category", "Azure Developer"},
                    {"Level", "Advanced"},
                    {"ThumbnailUrl", "https://stvidstrg25.blob.core.windows.net/skilltechimages/azure-devops-400.webp"},
                    {"VideoUrl", "https://www.youtube.com/embed/U3aFSfg64fQ"},
                    {"DurationMinutes", "418"},
                    {"Price", "₹7999"},
                    {"Enrollments", "0"},
                    {"Rating", "4.8"}
                },
                new Dictionary<string, string>
                {
                    {"Title", "Azure AI Agent Certification"},
                    {"Description", "Build intelligent AI agents using Azure AI services. Create autonomous agents, implement reasoning, and deploy production-ready solutions."},
                    {"Category", "Azure AI"},
                    {"Level", "Intermediate"},
                    {"ThumbnailUrl", "https://stvidstrg25.blob.core.windows.net/skilltechimages/azure-ai-agent.webp"},
                    {"VideoUrl", "https://www.youtube.com/embed/2wYLr9j3ktU"},
                    {"DurationMinutes", "388"},
                    {"Price", "Free"},
                    {"Enrollments", "0"},
                    {"Rating", "4.8"}
                }
            };
        }
    }
}
