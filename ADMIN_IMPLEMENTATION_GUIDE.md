# Admin Authentication System - Complete Implementation Guide

## 🎯 Overview

This is a production-ready admin authentication system for the Maruti Training Portal built with ASP.NET Core 8 and Identity. The system follows enterprise DevOps best practices with zero hardcoded credentials, free-tier-only dependencies, and comprehensive security features.

## 🏗️ Architecture

### Areas Pattern
```
/Admin/*                    → Protected admin area (requires Admin role)
/Trainings, /Courses, etc.  → Public pages (anonymous access)
```

### Key Components

1. **ASP.NET Core Identity**: Cookie-based authentication with role-based authorization
2. **Admin Seeding**: Automatic admin user creation from User Secrets/env vars (no hardcoded credentials)
3. **Security Services**:
   - `IHtmlSanitizerService`: XSS prevention using Ganss.XSS
   - `IRateLimitService`: Sliding window rate limiter (in-memory, Redis-ready)
4. **Admin Dashboard**: Statistics cards + Chart.js visualizations
5. **CI/CD**: GitHub Actions workflow for automated deployment
6. **Testing**: xUnit tests for authentication flows

## 📂 Project Structure

```
Areas/
  Admin/
    Controllers/
      AccountController.cs         # Login/Logout
      DashboardController.cs       # Admin dashboard with stats
    Views/
      Account/
        Login.cshtml               # Standalone login page
        AccessDenied.cshtml        # 403 error page
      Dashboard/
        Index.cshtml               # Dashboard with Chart.js
      Shared/
        _AdminLayout.cshtml        # Admin sidebar layout
      _ViewStart.cshtml
      _ViewImports.cshtml
    Models/
      ViewModels.cs                # LoginViewModel, AdminDashboardViewModel
Controllers/
  TrainingsController.cs           # ✅ Updated with [Authorize], HTML sanitization
  CoursesController.cs             # ✅ Updated with [Authorize], HTML sanitization
  EventsController.cs              # ✅ Updated with [Authorize], HTML sanitization
  ContactController.cs             # ✅ Updated with rate limiting
  ProfileController.cs             # ✅ Updated with [Authorize], HTML sanitization
Helpers/
  AdminSeeder.cs                   # Seeds admin from User Secrets/env vars
Services/
  HtmlSanitizerService.cs          # XSS prevention (Ganss.XSS wrapper)
  RateLimitService.cs              # In-memory sliding window rate limiter
Middleware/
  SimpleAdminAuth.cs               # Optional lightweight alternative to Identity
Tests/
  AdminAuthenticationTests.cs      # xUnit tests for admin auth
  MarutiTrainingPortal.Tests.csproj
.github/
  workflows/
    ci-deploy-admin.yml            # GitHub Actions CI/CD
README_ADMIN_SETUP.md              # Detailed setup instructions
appsettings.Development.json.example
appsettings.Production.json.example
```

## 🚀 Quick Start (Development)

### 1. Set Admin Credentials (User Secrets)

```powershell
# Navigate to project directory
cd c:\maruti-makwana

# Set admin email and password
dotnet user-secrets set "Admin:Email" "admin@marutitraining.com"
dotnet user-secrets set "Admin:Password" "SecurePassword123!"

# Verify
dotnet user-secrets list
```

### 2. Run the Application

```powershell
dotnet run
```

### 3. Access Admin Area

Navigate to: `http://localhost:5204/Admin/Account/Login`

Login with the credentials set in User Secrets.

## 🔒 Security Features

### Authentication & Authorization
- ✅ **Cookie-based authentication** (ASP.NET Core Identity)
- ✅ **Role-based authorization** (`[Authorize(Roles="Admin")]`)
- ✅ **Server-side role checks** in controller actions (`if (!User.IsInRole("Admin")) return Forbid();`)
- ✅ **Password policies**: 8+ chars, uppercase, lowercase, digit, special char
- ✅ **Lockout**: 15 minutes after 5 failed attempts
- ✅ **Anti-forgery tokens** (`[ValidateAntiForgeryToken]`)

### Input Sanitization
- ✅ **HTML sanitization** before database saves (Ganss.XSS)
- ✅ **Allowed tags**: `p, br, strong, em, u, h1-h3, ul, ol, li, a`
- ✅ **Dangerous attributes removed**: `onclick, onerror, javascript:`

### Rate Limiting
- ✅ **Contact form**: 3 requests per 10 minutes per IP
- ✅ **Sliding window algorithm** (prevents burst attacks)
- ✅ **Production upgrade path** to Redis for distributed rate limiting

### Credential Management
- ✅ **Development**: User Secrets (never committed to git)
- ✅ **Production**: Environment variables (Azure App Service, Docker, etc.)
- ✅ **No hardcoded credentials** anywhere in code

## 🧪 Testing

### Run Unit Tests

```powershell
# Navigate to test project
cd c:\maruti-makwana\Tests

# Run tests
dotnet test

# Run with detailed output
dotnet test --verbosity detailed

# Run with code coverage
dotnet test /p:CollectCoverage=true /p:CoverageReporter=html
```

### Test Coverage
- ✅ Admin user seeding creates user and role
- ✅ Login succeeds with correct credentials
- ✅ Login fails with incorrect password
- ✅ Login fails for non-existent user
- ✅ Account locks after 5 failed attempts
- ✅ Seeding is idempotent (no duplicates)
- ✅ Admin user has email confirmed

## 📊 Admin Dashboard Features

### Statistics Cards
- Total Trainings
- Total Courses
- Total Events
- Total Contact Messages

### Charts (Chart.js 4.4.0)
1. **Monthly Trainings** - Line chart showing trainings per month (last 6 months)
2. **Courses by Category** - Doughnut chart showing course distribution

### Recent Activity
- Last 5 contact messages
- Upcoming events (next 5)

### Navigation
- Manage Trainings
- Manage Courses
- Manage Events
- View Contact Messages
- Edit Profile
- View Public Site
- Logout

## 🌍 Production Deployment

### Option 1: Azure App Service (Free Tier Available)

1. **Create Azure App Service**
   ```powershell
   az webapp create --name marutitraining --resource-group myResourceGroup --plan myAppServicePlan --runtime "DOTNET|8.0"
   ```

2. **Set Environment Variables** (Azure Portal → Configuration → Application settings)
   ```
   ASPNETCORE_Admin__Email = admin@marutitraining.com
   ASPNETCORE_Admin__Password = SecurePassword123!
   ConnectionStrings__DefaultConnection = Server=tcp:yourserver.database.windows.net,1433;...
   ```

3. **Deploy via GitHub Actions**
   - Add `AZURE_PUBLISH_PROFILE` secret to GitHub repo
   - Update `.github/workflows/ci-deploy-admin.yml` with your app name
   - Push to `main` branch

### Option 2: Docker

1. **Create Dockerfile** (if not exists)
   ```dockerfile
   FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
   WORKDIR /app
   EXPOSE 80

   FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
   WORKDIR /src
   COPY ["MarutiTrainingPortal.csproj", "./"]
   RUN dotnet restore
   COPY . .
   RUN dotnet publish -c Release -o /app/publish

   FROM base AS final
   WORKDIR /app
   COPY --from=build /app/publish .
   ENTRYPOINT ["dotnet", "MarutiTrainingPortal.dll"]
   ```

2. **Set Environment Variables** (`.env` file - DO NOT COMMIT)
   ```env
   ASPNETCORE_Admin__Email=admin@marutitraining.com
   ASPNETCORE_Admin__Password=SecurePassword123!
   ConnectionStrings__DefaultConnection=Server=db;Database=MarutiTraining;...
   ```

3. **Run with Docker Compose**
   ```yaml
   version: '3.8'
   services:
     web:
       build: .
       environment:
         - ASPNETCORE_Admin__Email=${ASPNETCORE_Admin__Email}
         - ASPNETCORE_Admin__Password=${ASPNETCORE_Admin__Password}
       ports:
         - "5000:80"
   ```

   ```powershell
   docker compose up -d
   ```

## 🔄 Database Migrations

### SQLite (Development)
```powershell
# Default - no action needed
dotnet run
```

### SQL Server (Production)

1. **Update Connection String** (environment variable)
   ```
   ConnectionStrings__DefaultConnection=Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=MarutiTraining;Persist Security Info=False;User ID=yourusername;Password=yourpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
   ```

2. **Apply Migrations**
   ```powershell
   dotnet ef database update --connection "YourConnectionString"
   ```

## 🛠️ Customization

### Change Password Requirements
Edit `Program.cs`:
```csharp
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 12; // Change from 8
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
})
```

### Change Lockout Duration
```csharp
options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30); // Change from 15
options.Lockout.MaxFailedAccessAttempts = 3; // Change from 5
```

### Change Rate Limiting
Edit `ContactController.cs`:
```csharp
if (!_rateLimiter.IsAllowed(identifier, maxRequests: 5, window: TimeSpan.FromMinutes(15)))
```

### Add More Sanitization Tags
Edit `Services/HtmlSanitizerService.cs`:
```csharp
_sanitizer.AllowedTags.Add("blockquote");
_sanitizer.AllowedTags.Add("code");
```

## 🔍 Troubleshooting

### "Admin user not created on startup"
**Check:**
1. User Secrets configured: `dotnet user-secrets list`
2. Password meets complexity requirements
3. Application logs for errors

**Solution:**
```powershell
# Delete database and re-seed
Remove-Item MarutiTrainingPortal.db
dotnet run
```

### "Invalid login attempt"
**Check:**
1. Correct credentials in User Secrets/env vars
2. Admin user exists in `AspNetUsers` table
3. Admin role assigned in `AspNetUserRoles` table
4. Account not locked (wait 15 minutes or reset in database)

### "Environment variables not loading in Azure"
**Check:**
1. Restart app after setting variables
2. Verify in **Configuration → Application settings**
3. Correct naming: `ASPNETCORE_Admin__Email` (double underscore)

### Tests failing
```powershell
# Restore test dependencies
cd Tests
dotnet restore
dotnet build

# Run tests with verbose output
dotnet test --verbosity detailed
```

## 📚 Additional Resources

- **Admin Setup Guide**: [README_ADMIN_SETUP.md](README_ADMIN_SETUP.md)
- **ASP.NET Core Identity**: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity
- **User Secrets**: https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets
- **Azure Deployment**: https://learn.microsoft.com/en-us/azure/app-service/

## 🆚 Alternative: Lightweight SimpleAdminAuth

If you prefer a simpler authentication system without Identity:

**File**: `Middleware/SimpleAdminAuth.cs`

**Features**:
- ✅ PBKDF2 password hashing (100,000 iterations)
- ✅ Cookie-based authentication
- ✅ Reads credentials from User Secrets/env vars
- ✅ No database tables for users/roles

**Limitations**:
- ❌ No lockout protection
- ❌ No email confirmation
- ❌ No password reset
- ❌ No two-factor auth

**Setup**:
```csharp
// In Program.cs (replace Identity code)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => { /* ... */ });

app.UseSimpleAdminAuth();
```

See `Middleware/SimpleAdminAuth.cs` for full implementation and setup instructions.

## 📝 License

MIT License - See main [README.md](README.md)

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/YourFeature`)
3. Commit changes (`git commit -m 'Add YourFeature'`)
4. Push to branch (`git push origin feature/YourFeature`)
5. Open Pull Request

---

**Version**: 1.0.0  
**Last Updated**: 2025  
**Maintained By**: Maruti Training Portal Team
