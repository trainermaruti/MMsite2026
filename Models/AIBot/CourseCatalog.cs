namespace MarutiTrainingPortal.Models
{
    public class CourseCatalog
    {
        public string Version { get; set; } = string.Empty;
        public string LastUpdated { get; set; } = string.Empty;
        public List<AICourse> Courses { get; set; } = new();
        public List<Product> Products { get; set; } = new();
        public List<LearningPath> LearningPaths { get; set; } = new();
        public List<Certification> Certifications { get; set; } = new();
        public CatalogMetadata Metadata { get; set; } = new();
    }

    public class AICourse
    {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public List<string> Prerequisites { get; set; } = new();
        public string Description { get; set; } = string.Empty;
        public List<string> Outcomes { get; set; } = new();
        public List<string> TargetRoles { get; set; } = new();
        public List<string> Modules { get; set; } = new();
        public string? CertificationExam { get; set; }
        public List<string> NextSteps { get; set; } = new();
        public string Url { get; set; } = string.Empty;
        public bool IsFree { get; set; }
        public bool IsPremium { get; set; }
        public bool RequiresPrerequisiteCertification { get; set; }
        public List<string> PrerequisiteCertifications { get; set; } = new();
    }

    public class Product
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Features { get; set; } = new();
        public string Url { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public List<string> IncludedIn { get; set; } = new();
    }

    public class LearningPath
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Courses { get; set; } = new();
        public string EstimatedDuration { get; set; } = string.Empty;
        public string TargetAudience { get; set; } = string.Empty;
    }

    public class Certification
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public List<string> RequiredExams { get; set; } = new();
        public List<string> Prerequisites { get; set; } = new();
    }

    public class CatalogMetadata
    {
        public int TotalCourses { get; set; }
        public int FreeCourses { get; set; }
        public int PremiumCourses { get; set; }
        public int FundamentalCertifications { get; set; }
        public int AssociateCertifications { get; set; }
        public int ExpertCertifications { get; set; }
    }
}
