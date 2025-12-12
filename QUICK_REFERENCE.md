# 🚀 Developer Quick Reference - Enterprise Features

## 📍 File Locations

### Models
```
Models/Certificate.cs          - Certificate entity with revocation
Models/LeadAuditLog.cs        - Audit log for lead changes
Models/ContactMessage.cs      - Enhanced with EventId, Status
Models/Training.cs            - Updated with soft delete, audit
Models/Course.cs              - Updated with soft delete, audit
Models/TrainingEvent.cs       - Updated with soft delete, audit
```

### Controllers
```
Controllers/VerifyController.cs   - Certificate verification (PUBLIC)
Controllers/SitemapController.cs  - Dynamic sitemap.xml
```

### Services
```
Services/StatService.cs           - Cached statistics
Services/ChatbotService.cs        - AI chatbot (free/paid modes)
Middleware/RateLimitMiddleware.cs - Rate limiting protection
```

### Views
```
Views/Verify/Index.cshtml         - Certificate verification page
```

### Styles
```
wwwroot/css/enterprise.css        - Enterprise UI theme
```

### Config
```
appsettings.Development.json.example  - Configuration template
```

### Documentation
```
README_FEATURES.md                    - Full implementation guide
IMPLEMENTATION_SUMMARY_ENTERPRISE.md  - Summary & next steps
```

---

## 🔌 API Endpoints

### Public (No Auth)
| Method | Endpoint | Description | Rate Limit |
|--------|----------|-------------|------------|
| GET | /Verify | Verification page | No |
| POST | /Verify/Check | Verify certificate | 10/min |
| GET | /sitemap.xml | Dynamic sitemap | No |
| GET | /api/events/calendar | Events JSON | No |
| POST | /Contact | Contact form | 3/min |

### Admin (Requires Auth)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /Admin/Certificates | Manage certificates |
| GET | /Admin/Leads | Lead management (to implement) |
| POST | /Admin/Leads/UpdateStatus | Update lead status (to implement) |
| GET | /Admin/Leads/ExportCsv | Export CSV (to implement) |

---

## 💾 Database Schema Updates

### New Tables
```sql
Certificates
  - Id, CertificateId (unique), StudentName, StudentEmail
  - CourseTitle, CourseCategory, CompletionDate, IssueDate
  - Instructor, DurationHours, Score, Grade
  - IsRevoked, RevokedDate, RevocationReason
  - CreatedDate, UpdatedDate, IsDeleted

LeadAuditLogs
  - Id, ContactMessageId, Action, OldValue, NewValue
  - ChangedBy, Notes, CreatedDate
```

### Updated Tables (All)
```sql
ALTER TABLE [Trainings|Courses|TrainingEvents|ContactMessages]
  ADD UpdatedDate datetime2 NULL
  ADD IsDeleted bit NOT NULL DEFAULT 0
  
ContactMessages:
  ADD EventId int NULL
  ADD Status nvarchar(50) NOT NULL DEFAULT 'New'
```

---

## ⚙️ Configuration

### appsettings.json
```json
{
  "Chatbot": {
    "Mode": "LocalMock",  // or "OpenAI"
    "ApiKey": "",         // Optional: for OpenAI mode
    "Endpoint": ""        // Optional: for Azure OpenAI
  },
  "RateLimit": {
    "Contact": { "WindowSeconds": 60, "MaxRequests": 3 },
    "Verify": { "WindowSeconds": 60, "MaxRequests": 10 },
    "Event": { "WindowSeconds": 300, "MaxRequests": 5 }
  }
}
```

### User Secrets (Development)
```bash
dotnet user-secrets set "Chatbot:ApiKey" "your-key-here"
dotnet user-secrets set "Chatbot:Endpoint" "https://your-resource.openai.azure.com/"
```

### Environment Variables (Production)
```bash
Chatbot__Mode=LocalMock
Chatbot__ApiKey=<from-key-vault>
RateLimit__Verify__MaxRequests=20
```

---

## 🎨 CSS Classes

### Enterprise UI
```css
.gradient-text         - Gradient heading (use sparingly)
.hero-background       - Grid background with glow
.card-enterprise       - Professional card style
.hover-lift            - Card with hover effect
.skeleton              - Loading skeleton
.skeleton-text         - Text skeleton loader
.btn-enterprise        - Enterprise button
.form-control-modern   - Modern input style
.avatar-wrapper        - Avatar with ring
.badge-enterprise      - Professional badge
.table-enterprise      - Professional table
.alert-enterprise      - Matte alert (no glow)
```

### Color Variables
```css
--color-primary: #6366f1
--color-secondary: #8b5cf6
--color-bg-dark: #0f172a
--color-bg-slate: #1e293b
--color-text-primary: #f1f5f9
--radius-lg: 12px
--shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.2)
```

---

## 🧪 Testing

### Test Certificate IDs
```
CERT-2024-001234  - John Doe, Azure Fundamentals (Valid)
CERT-2024-001235  - Jane Smith, AI and Machine Learning (Valid)
CERT-2024-001236  - Bob Johnson, DevOps Essentials (Valid)
```

### Manual Testing Steps
```bash
# 1. Run migrations
dotnet ef migrations add EnterpriseFeatures
dotnet ef database update

# 2. Start app
dotnet run

# 3. Test verification
Navigate to: https://localhost:5204/Verify
Enter: CERT-2024-001234
Observe: Skeleton loader → Valid certificate details

# 4. Test rate limiting
Submit contact form 4 times rapidly
Observe: 4th attempt returns 429 Too Many Requests

# 5. Test sitemap
Navigate to: https://localhost:5204/sitemap.xml
Observe: XML sitemap with all public pages
```

---

## 🔧 Common Commands

### Database
```bash
# Create migration
dotnet ef migrations add MigrationName

# Apply migration
dotnet ef database update

# Rollback migration
dotnet ef database update PreviousMigrationName

# Drop database
dotnet ef database drop
```

### Development
```bash
# Run app
dotnet run

# Watch mode (auto-reload)
dotnet watch run

# Build
dotnet build

# Clean
dotnet clean
```

### Secrets Management
```bash
# Set secret
dotnet user-secrets set "Key:Subkey" "value"

# List secrets
dotnet user-secrets list

# Remove secret
dotnet user-secrets remove "Key:Subkey"

# Clear all secrets
dotnet user-secrets clear
```

---

## 🐛 Troubleshooting

### Issue: Certificate not found
**Solution:** Run migration script to seed sample data

### Issue: Rate limit too strict
**Solution:** Increase limits in appsettings.json

### Issue: Chatbot not working
**Solution:** Check mode in appsettings, verify API key if using OpenAI

### Issue: Skeleton loader not showing
**Solution:** Ensure enterprise.css is referenced in layout

### Issue: Soft delete showing deleted records
**Solution:** DbContext automatically filters, check if using IgnoreQueryFilters()

---

## 📦 NuGet Packages Needed

### Current
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.11" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.11" />
<PackageReference Include="Ganss.Xss" Version="3.2.13" />
```

### To Implement Remaining Features
```bash
# For CSV export
dotnet add package CsvHelper

# For Excel export
dotnet add package EPPlus

# For testing
dotnet add package Microsoft.EntityFrameworkCore.InMemory
dotnet add package xunit
dotnet add package Moq
```

---

## 🚀 Next Steps

1. ✅ Review this quick reference
2. ✅ Run `dotnet ef database update`
3. ✅ Test `/Verify` with sample certificate IDs
4. ⏭️ Optional: Implement Calendar (FullCalendar)
5. ⏭️ Optional: Implement Lead Management
6. ⏭️ Optional: Implement Chatbot UI
7. ⏭️ Optional: Write xUnit tests

---

## 📝 Code Snippets

### Using StatService in Controller
```csharp
public class HomeController : Controller
{
    private readonly StatService _statService;

    public HomeController(StatService statService)
    {
        _statService = statService;
    }

    public async Task<IActionResult> Index()
    {
        var stats = await _statService.GetSiteStatisticsAsync();
        return View(stats);
    }
}
```

### Using Chatbot Service
```csharp
public class ChatController : Controller
{
    private readonly ChatbotService _chatbot;

    public async Task<IActionResult> Ask(string message)
    {
        var response = await _chatbot.GetResponseAsync(message);
        return Json(response);
    }
}
```

### Soft Delete Pattern
```csharp
// Soft delete
training.IsDeleted = true;
training.UpdatedDate = DateTime.UtcNow;
await _context.SaveChangesAsync();

// Restore
training.IsDeleted = false;
training.UpdatedDate = DateTime.UtcNow;
await _context.SaveChangesAsync();

// Query (automatically excludes soft deleted)
var trainings = await _context.Trainings.ToListAsync();

// Include deleted (when needed)
var all = await _context.Trainings
    .IgnoreQueryFilters()
    .ToListAsync();
```

---

**Last Updated:** December 1, 2025  
**Version:** 1.3 Enterprise  
**Status:** Ready for Development
