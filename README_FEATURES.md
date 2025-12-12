# Enterprise Features Implementation Guide

## Overview

This document describes the new enterprise-grade features added to Maruti Training Portal:

1. **Certificate Verification** - Public verification of training certificates
2. **Live Training Calendar** - FullCalendar integration with event registration
3. **Enhanced Lead Management** - Admin tools for contact/lead tracking with CSV export
4. **Caching & Performance** - IMemoryCache for statistics, dynamic sitemap
5. **AI Chatbot** (Optional) - Free local mock or paid OpenAI integration
6. **Rate Limiting** - Protection against API abuse
7. **Enterprise UI** - Modern dark theme with skeleton loaders

---

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server LocalDB (or SQLite as fallback)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone and restore packages:**
```bash
cd c:\maruti-makwana
dotnet restore
```

2. **Apply database migrations:**

Option A - Using Entity Framework CLI:
```bash
dotnet ef migrations add EnterpriseFeatures
dotnet ef database update
```

Option B - Using SQL script:
```bash
# Run the migration script in SQL Server Management Studio or via command line
sqlcmd -S "(localdb)\mssqllocaldb" -d MarutiTrainingPortalDb -i Migrations\AddEnterpriseFeatures.sql
```

3. **Configure appsettings (optional features):**

Create `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MarutiTrainingPortalDb;Trusted_Connection=true;"
  },
  "Chatbot": {
    "Mode": "LocalMock",
    "ApiKey": "",
    "Endpoint": ""
  },
  "RateLimit": {
    "Contact": {
      "WindowSeconds": 60,
      "MaxRequests": 3
    },
    "Verify": {
      "WindowSeconds": 60,
      "MaxRequests": 10
    },
    "Event": {
      "WindowSeconds": 300,
      "MaxRequests": 5
    }
  }
}
```

4. **Seed sample data (optional):**
```bash
# Sample certificates are automatically seeded via migration script
# To add more, use the admin panel at /Admin/Certificates
```

5. **Run the application:**
```bash
dotnet run
```

Navigate to `https://localhost:5204`

---

## 📜 Certificate Verification

### Public Access (No Auth Required)

**Endpoint:** `GET /Verify`

Users can verify certificates by entering a Certificate ID.

**Test Certificate IDs:**
- `CERT-2024-001234` - John Doe, Azure Fundamentals
- `CERT-2024-001235` - Jane Smith, AI and Machine Learning
- `CERT-2024-001236` - Bob Johnson, DevOps Essentials

### API Endpoint

**POST** `/Verify/Check`

Request:
```json
{
  "certificateId": "CERT-2024-001234"
}
```

Response (Success):
```json
{
  "success": true,
  "message": "Certificate verified successfully!",
  "certificateData": {
    "certificateId": "CERT-2024-001234",
    "studentName": "John Doe",
    "courseTitle": "Azure Fundamentals",
    "courseCategory": "Cloud",
    "completionDate": "2024-11-15T00:00:00",
    "issueDate": "2024-11-16T00:00:00",
    "instructor": "Maruti Makwana",
    "durationHours": 40,
    "score": 95.5,
    "grade": "A+",
    "status": "Valid"
  }
}
```

Response (Not Found):
```json
{
  "success": false,
  "message": "Certificate not found. Please check the Certificate ID and try again."
}
```

### Features:
- ✅ Skeleton loader while fetching
- ✅ Rate limited (10 requests per minute per IP)
- ✅ Shows revoked certificates with reason
- ✅ Enterprise dark UI with gradient design

---

## 📅 Live Training Calendar

### Integration

The calendar uses **FullCalendar** (free MIT license) to display upcoming training events.

**Page:** `/Events/Calendar`

### Events API

**GET** `/api/events/calendar`

Returns events in FullCalendar format:
```json
[
  {
    "id": 1,
    "title": "Azure Cloud Workshop",
    "start": "2024-12-15T09:00:00",
    "end": "2024-12-15T17:00:00",
    "url": "/Events/Details/1",
    "backgroundColor": "#6366f1",
    "borderColor": "#4f46e5"
  }
]
```

### Register Interest

Users can click "Register Interest" on calendar events, which creates a ContactMessage with:
- EventId linked to the training event
- Status set to "New"
- Captured via rate-limited endpoint

---

## 📊 Admin Lead Management

### Access

**URL:** `/Admin/Leads` (Requires Admin role)

### Features

1. **Status Management:**
   - New
   - Contacted
   - Qualified
   - Converted
   - Closed

2. **Bulk Export:**
   - CSV format (free, using CsvHelper)
   - XLSX format (free, using EPPlus Community Edition)

3. **Audit Logging:**
   - Every status change creates a LeadAuditLog entry
   - Tracks who changed what and when

### API Endpoints

**POST** `/Admin/Leads/UpdateStatus`
```json
{
  "leadId": 123,
  "newStatus": "Contacted",
  "notes": "Called and left voicemail"
}
```

**GET** `/Admin/Leads/ExportCsv`

Downloads CSV file with all leads

**GET** `/Admin/Leads/ExportExcel`

Downloads XLSX file with all leads

---

## ⚡ Performance & Caching

### Statistics Caching

`StatService` caches homepage statistics for 1 hour using `IMemoryCache`.

```csharp
// Inject in controllers
private readonly StatService _statService;

// Use in actions
var stats = await _statService.GetSiteStatisticsAsync();
```

**Scaling Note:** For multi-server deployments, replace with Redis:
```csharp
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});
```

### Dynamic Sitemap

**URL:** `/sitemap.xml`

Automatically generates sitemap including:
- All static pages
- Training details pages
- Course details pages
- Recent event pages

Cached for 1 hour for performance.

---

## 🤖 AI Chatbot (Optional)

### Two Modes

#### 1. LocalMock (Free, Default)

Uses seeded Q&A responses. No API keys required.

**Configure:**
```json
{
  "Chatbot": {
    "Mode": "LocalMock"
  }
}
```

#### 2. OpenAI (Paid, Optional)

**⚠️ OPTIONAL: May incur costs**

Requires Azure OpenAI or OpenAI API key.

**For Azure OpenAI:**
```json
{
  "Chatbot": {
    "Mode": "OpenAI",
    "ApiKey": "your-azure-openai-key",
    "Endpoint": "https://your-resource.openai.azure.com/"
  }
}
```

**For OpenAI:**
```json
{
  "Chatbot": {
    "Mode": "OpenAI",
    "ApiKey": "sk-your-openai-key",
    "Endpoint": null
  }
}
```

**Using dotnet user-secrets (recommended for development):**
```bash
dotnet user-secrets set "Chatbot:ApiKey" "your-key-here"
dotnet user-secrets set "Chatbot:Endpoint" "https://your-resource.openai.azure.com/"
```

### Privacy Notice

The chatbot includes a privacy consent checkbox. Users must agree before using AI features.

---

## 🛡️ Rate Limiting

### Configuration

Default limits (in-memory sliding window):

| Endpoint | Window | Max Requests |
|----------|--------|--------------|
| /Contact | 60s | 3 |
| /Verify/Check | 60s | 10 |
| /api/events/register | 300s | 5 |

### Customize

In `appsettings.json`:
```json
{
  "RateLimit": {
    "Contact": {
      "WindowSeconds": 60,
      "MaxRequests": 3
    }
  }
}
```

### Scaling Note

For production with multiple servers, replace in-memory storage with Redis:

1. Install package:
```bash
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
```

2. Modify middleware to use `IDistributedCache` instead of static dictionary

---

## 🎨 Enterprise UI Styles

### Design System

**Font:** Inter (loaded from Google Fonts)

**Color Palette:**
- Primary: `#6366f1` (Indigo)
- Secondary: `#8b5cf6` (Purple)
- Background: `#0f172a` (Navy), `#1e293b` (Slate)
- Text: `#f1f5f9` (White), `#cbd5e1` (Gray)

**Border Radius:** 8-12px (enterprise, not gaming)

**Shadows:** Soft, subtle (no neon glows)

### CSS Classes

```css
/* Gradient text (use sparingly, only for main headings) */
.gradient-text {
  background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

/* Card with hover lift */
.hover-lift {
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.hover-lift:hover {
  transform: translateY(-4px);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
}

/* Skeleton loader */
.skeleton {
  background: linear-gradient(90deg, #334155 0%, #475569 50%, #334155 100%);
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
  border-radius: 4px;
}

@keyframes shimmer {
  0% { background-position: -200% 0; }
  100% { background-position: 200% 0; }
}
```

### Hero Section Grid Background

```css
.hero-background {
  background-image: 
    linear-gradient(rgba(99, 102, 241, 0.05) 1px, transparent 1px),
    linear-gradient(90deg, rgba(99, 102, 241, 0.05) 1px, transparent 1px);
  background-size: 40px 40px;
}
```

---

## 🧪 Testing

### Running Tests

```bash
dotnet test
```

### Test Coverage

Tests included for:
- ✅ VerifyController (happy path, not found, revoked)
- ✅ LeadsController (status update, export)
- ✅ StatService (caching behavior)
- ✅ RateLimitMiddleware (blocking after limit)

### Sample Test (xUnit + InMemory EF)

```csharp
[Fact]
public async Task VerifyController_ValidCertificate_ReturnsSuccess()
{
    // Arrange
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase("TestDb")
        .Options;

    using var context = new ApplicationDbContext(options);
    context.Certificates.Add(new Certificate
    {
        CertificateId = "TEST-001",
        StudentName = "Test User",
        CourseTitle = "Test Course"
    });
    await context.SaveChangesAsync();

    var controller = new VerifyController(context, new NullLogger<VerifyController>());

    // Act
    var result = await controller.Check(new VerifyRequest { CertificateId = "TEST-001" });

    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result);
    var response = Assert.IsType<VerifyResponse>(okResult.Value);
    Assert.True(response.Success);
}
```

---

## 📦 Production Deployment

### Checklist

- [ ] Change default admin password (`Admin@123456`)
- [ ] Configure production connection string
- [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Configure HTTPS certificate
- [ ] Set up Redis for distributed caching (if multi-server)
- [ ] Set up Redis for distributed rate limiting (if multi-server)
- [ ] Configure email SMTP settings
- [ ] Review and update OpenAI keys (if using)
- [ ] Enable Application Insights or logging
- [ ] Configure backup strategy for database
- [ ] Test all endpoints with production data
- [ ] Update robots.txt and sitemap.xml URLs

### Environment Variables (Azure App Service)

```bash
# Connection String
CUSTOMCONNSTR_DefaultConnection=Server=...

# Chatbot (optional)
Chatbot__Mode=LocalMock
Chatbot__ApiKey=<from-key-vault>
Chatbot__Endpoint=<from-key-vault>

# Rate Limiting (customize for production load)
RateLimit__Contact__MaxRequests=5
RateLimit__Verify__MaxRequests=20
```

### GitHub Secrets (for CI/CD)

```yaml
# .github/workflows/deploy.yml
env:
  CHATBOT_API_KEY: ${{ secrets.CHATBOT_API_KEY }}
  CONNECTION_STRING: ${{ secrets.CONNECTION_STRING }}
```

---

## 🔐 Security

### Authentication & Authorization

- **Public Pages:** No auth required (/, /Trainings, /Courses, /Events, /Verify, /Contact)
- **Admin Pages:** Requires `[Authorize(Roles="Admin")]`
- **Write Operations:** Always verified server-side

### Rate Limiting

- Contact form: 3 requests/minute
- Verify endpoint: 10 requests/minute
- Event registration: 5 requests/5 minutes

### Input Validation

- All models use DataAnnotations
- HTML sanitization via HtmlSanitizerService
- CSRF protection enabled
- XSS prevention via Razor encoding

---

## 📚 API Reference

### Public Endpoints

| Method | Endpoint | Auth | Rate Limited | Description |
|--------|----------|------|--------------|-------------|
| GET | /Verify | No | No | Certificate verification page |
| POST | /Verify/Check | No | Yes (10/min) | Verify certificate by ID |
| GET | /Events/Calendar | No | No | Calendar view page |
| GET | /api/events/calendar | No | No | Get events JSON for FullCalendar |
| POST | /api/events/register | No | Yes (5/5min) | Register interest in event |
| GET | /sitemap.xml | No | No | Dynamic sitemap |
| POST | /Contact | No | Yes (3/min) | Submit contact form |

### Admin Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | /Admin/Leads | Admin | View all leads |
| POST | /Admin/Leads/UpdateStatus | Admin | Update lead status |
| GET | /Admin/Leads/ExportCsv | Admin | Export leads as CSV |
| GET | /Admin/Leads/ExportExcel | Admin | Export leads as XLSX |
| GET | /Admin/Certificates | Admin | Manage certificates |
| POST | /Admin/Certificates/Create | Admin | Issue new certificate |

---

## 🐛 Troubleshooting

### Database Migration Fails

```bash
# Reset migrations
dotnet ef database drop
dotnet ef migrations remove
dotnet ef migrations add Initial
dotnet ef database update
```

### Certificate Not Found

- Ensure migration ran successfully
- Check `Certificates` table has sample data
- Verify `IsDeleted = false` and `IsRevoked = false`

### Rate Limit Too Strict

Adjust in `appsettings.json`:
```json
{
  "RateLimit": {
    "Verify": {
      "MaxRequests": 50
    }
  }
}
```

### Chatbot Not Working

1. Check `appsettings.json` has correct mode
2. For OpenAI mode, verify API key is set
3. Check logs for error messages
4. Fallback to LocalMock if API issues

### Calendar Not Loading

1. Ensure FullCalendar CSS/JS are referenced in layout
2. Check browser console for JavaScript errors
3. Verify `/api/events/calendar` returns valid JSON
4. Check CORS settings if API is on different domain

---

## 📝 Sample Data

### Seed Events for Testing

Create `seed-events.json`:
```json
[
  {
    "title": "Azure Fundamentals Workshop",
    "description": "Learn Azure basics",
    "startDate": "2024-12-15T09:00:00",
    "endDate": "2024-12-15T17:00:00",
    "location": "Virtual",
    "eventType": "Workshop",
    "maxParticipants": 50
  }
]
```

Import via admin panel or seed in `Program.cs`.

---

## 🤝 Support

For issues or questions:
- Email: admin@marutitraining.com
- GitHub Issues: [Create Issue]
- Documentation: This file

---

## 📄 License

This project is proprietary. All rights reserved.

---

**Last Updated:** December 1, 2025  
**Version:** 1.3 Enterprise Edition
