# 🚀 Admin Authentication System - Deployment Checklist

Use this checklist to ensure your admin authentication system is properly configured before going to production.

## ✅ Development Setup (Local Machine)

### 1. User Secrets Configuration
- [ ] Run `dotnet user-secrets init`
- [ ] Set admin email: `dotnet user-secrets set "Admin:Email" "admin@marutitraining.com"`
- [ ] Set admin password: `dotnet user-secrets set "Admin:Password" "SecurePassword123!"`
- [ ] Verify: `dotnet user-secrets list`

### 2. Test Application Locally
- [ ] Run `dotnet run`
- [ ] Navigate to `http://localhost:5204`
- [ ] Verify public pages work (Trainings, Courses, Events, Contact)
- [ ] Navigate to `http://localhost:5204/Admin/Account/Login`
- [ ] Login with admin credentials
- [ ] Verify redirect to `/Admin/Dashboard`
- [ ] Check dashboard displays statistics and charts
- [ ] Test CRUD operations (Create/Edit/Delete) require admin login
- [ ] Test logout returns to public home page

### 3. Run Unit Tests
- [ ] Navigate to `Tests` directory
- [ ] Run `dotnet restore`
- [ ] Run `dotnet test`
- [ ] Verify all 9 tests pass

### 4. Security Verification
- [ ] Verify contact form rate limiting (submit 4 times quickly → should block)
- [ ] Verify failed login lockout (fail 5 times → locked for 15 min)
- [ ] Verify HTML sanitization (try adding `<script>` tag → should be stripped)
- [ ] Verify [AllowAnonymous] on public pages (access without login)
- [ ] Verify [Authorize] on admin pages (redirect to login)

## ✅ Production Preparation

### 1. Database Setup
- [ ] **SQLite (simple)**: Ensure `ConnectionStrings__DefaultConnection` is set to SQLite path
- [ ] **SQL Server (recommended)**: Create database in Azure SQL or on-prem
- [ ] Apply migrations: `dotnet ef database update --connection "YourConnectionString"`
- [ ] Verify database tables created (AspNetUsers, AspNetRoles, Trainings, Courses, etc.)

### 2. Azure App Service Deployment

#### Create App Service
- [ ] Login to Azure Portal
- [ ] Create new App Service (Runtime: .NET 8)
- [ ] Note the app service name (e.g., `marutitraining`)

#### Configure Environment Variables
- [ ] Navigate to **Configuration → Application settings**
- [ ] Add the following settings:

| Name | Value | Example |
|------|-------|---------|
| `ASPNETCORE_Admin__Email` | Your admin email | `admin@marutitraining.com` |
| `ASPNETCORE_Admin__Password` | Your admin password | `SecurePassword123!` |
| `ConnectionStrings__DefaultConnection` | SQL Server connection string | `Server=tcp:yourserver.database.windows.net,1433;...` |

- [ ] Click **Save**
- [ ] Restart the app service

#### Download Publish Profile
- [ ] In Azure Portal → App Service → **Get publish profile**
- [ ] Download the `.PublishSettings` file
- [ ] Copy the entire file content

### 3. GitHub Repository Setup

#### Add GitHub Secrets
- [ ] Navigate to your GitHub repository
- [ ] Go to **Settings → Secrets and variables → Actions**
- [ ] Click **New repository secret**
- [ ] Add the following secrets:

| Name | Value |
|------|-------|
| `AZURE_PUBLISH_PROFILE` | Paste entire publish profile content |

#### Update Workflow File
- [ ] Edit `.github/workflows/ci-deploy-admin.yml`
- [ ] Replace `your-azure-app-service-name` with your actual app service name
- [ ] Commit and push changes

### 4. Deploy to Production

#### Push to Main Branch
- [ ] Commit all changes: `git add .`
- [ ] Commit: `git commit -m "Deploy admin authentication system"`
- [ ] Push to main: `git push origin main`
- [ ] Navigate to GitHub → **Actions** tab
- [ ] Monitor deployment progress
- [ ] Verify "Build and Test" job passes
- [ ] Verify "Deploy to Azure" job succeeds

#### Verify Deployment
- [ ] Navigate to `https://your-app-name.azurewebsites.net`
- [ ] Verify public pages load
- [ ] Navigate to `https://your-app-name.azurewebsites.net/Admin/Account/Login`
- [ ] Login with production admin credentials
- [ ] Verify dashboard displays correctly
- [ ] Test CRUD operations
- [ ] Check contact form works and is rate-limited

## ✅ Docker Deployment (Alternative)

### 1. Prepare Environment
- [ ] Copy `.env.example` to `.env`
- [ ] Edit `.env` with your credentials:
  ```env
  ASPNETCORE_Admin__Email=admin@marutitraining.com
  ASPNETCORE_Admin__Password=SecurePassword123!
  ConnectionStrings__DefaultConnection=Data Source=/app/data/MarutiTrainingPortal.db
  ```
- [ ] **DO NOT COMMIT `.env` FILE!** (already in `.gitignore`)

### 2. Build and Run
- [ ] Run `docker compose up -d`
- [ ] Verify container started: `docker compose ps`
- [ ] View logs: `docker compose logs -f`
- [ ] Access app: `http://localhost:5000`
- [ ] Access admin: `http://localhost:5000/Admin/Account/Login`

### 3. Push to Docker Hub (Optional)
- [ ] Login: `docker login`
- [ ] Tag image: `docker tag marutitraining:latest yourusername/marutitraining:latest`
- [ ] Push: `docker push yourusername/marutitraining:latest`
- [ ] Add `DOCKER_USERNAME` and `DOCKER_PASSWORD` secrets to GitHub
- [ ] Push to main branch to trigger Docker build

## ✅ Security Hardening

### 1. Password Policies
- [ ] Verify password requirements enforced (8+ chars, uppercase, lowercase, digit, special)
- [ ] Test weak passwords are rejected
- [ ] Consider increasing to 12+ chars for production: Edit `Program.cs` → `options.Password.RequiredLength = 12;`

### 2. Lockout Policies
- [ ] Verify account locks after 5 failed attempts
- [ ] Verify lockout duration is 15 minutes
- [ ] Consider adjusting for your security needs: Edit `Program.cs` → `options.Lockout.*`

### 3. HTTPS Enforcement
- [ ] Azure App Service: Enable HTTPS-only in **Settings → TLS/SSL settings**
- [ ] Docker: Use reverse proxy (nginx, Traefik) with SSL certificates
- [ ] Test HTTP redirects to HTTPS

### 4. Rate Limiting
- [ ] Test contact form rate limiting (3 requests per 10 min)
- [ ] Consider Redis for production: Edit `Services/RateLimitService.cs` (instructions in comments)

### 5. HTML Sanitization
- [ ] Verify dangerous HTML is stripped from user input
- [ ] Test: Submit `<script>alert('XSS')</script>` in course description → should be removed
- [ ] Adjust allowed tags if needed: Edit `Services/HtmlSanitizerService.cs`

## ✅ Monitoring & Maintenance

### 1. Application Insights (Azure Only)
- [ ] Create Application Insights resource in Azure
- [ ] Copy instrumentation key
- [ ] Add to Azure App Service → **Configuration**:
  ```
  ASPNETCORE_ApplicationInsights__InstrumentationKey=your-key
  ```
- [ ] Monitor errors, performance, usage in Azure Portal

### 2. Database Backups
- [ ] **Azure SQL**: Enable automated backups (default: 7-day retention)
- [ ] **SQLite**: Schedule file backups (daily cron job)
- [ ] Test restore procedure

### 3. Credential Rotation
- [ ] Schedule password rotation (recommended: every 90 days)
- [ ] Update environment variables
- [ ] Delete old admin user from database (or use password reset)
- [ ] Restart application to re-seed with new password

### 4. Logs Monitoring
- [ ] Check application logs regularly
- [ ] Monitor failed login attempts
- [ ] Set up alerts for suspicious activity
- [ ] Consider integrating with logging service (Serilog + Azure App Insights)

## ✅ Documentation Review

### For Developers
- [ ] Read `ADMIN_IMPLEMENTATION_GUIDE.md` - Complete implementation guide
- [ ] Read `README_ADMIN_SETUP.md` - Detailed setup instructions
- [ ] Read `ADMIN_IMPLEMENTATION_SUMMARY.md` - What's been implemented

### For Operations
- [ ] Review `.env.example` - Environment variable template
- [ ] Review `appsettings.Production.json.example` - Configuration template
- [ ] Review `.github/workflows/ci-deploy-admin.yml` - CI/CD pipeline

### For Security Team
- [ ] Review `Services/HtmlSanitizerService.cs` - XSS prevention
- [ ] Review `Services/RateLimitService.cs` - Rate limiting implementation
- [ ] Review `Helpers/AdminSeeder.cs` - Credential management
- [ ] Review `Program.cs` - Password policies, lockout settings

## ✅ Final Checks

### Pre-Launch
- [ ] All environment variables set correctly
- [ ] Database connection string valid
- [ ] Admin user can login
- [ ] Public site accessible without login
- [ ] HTTPS enabled
- [ ] `.env` file NOT committed to git
- [ ] User Secrets NOT committed to git
- [ ] GitHub Actions workflow successful
- [ ] Unit tests passing

### Post-Launch
- [ ] Monitor application logs for errors
- [ ] Monitor failed login attempts
- [ ] Test contact form works
- [ ] Test admin CRUD operations
- [ ] Verify rate limiting works
- [ ] Verify HTML sanitization works
- [ ] Check Application Insights (if enabled)

## 🆘 Troubleshooting

### Application won't start
- Check environment variables are set correctly (double underscore in Azure: `Admin__Email`)
- Check database connection string is valid
- Check application logs for errors

### Admin user not created
- Verify User Secrets configured: `dotnet user-secrets list`
- Verify password meets requirements (8+ chars, uppercase, lowercase, digit, special)
- Delete database and restart app to re-seed

### Cannot login
- Verify credentials match User Secrets/environment variables
- Check for lockout (wait 15 minutes or reset in database)
- Verify admin user exists in `AspNetUsers` table
- Verify admin role assigned in `AspNetUserRoles` table

### Charts not loading
- Check browser console for JavaScript errors
- Verify Chart.js CDN is accessible
- Check `/Admin/Dashboard/GetChartData` returns valid JSON

### Contact form not rate limiting
- Verify `IRateLimitService` is registered in `Program.cs`
- Check `ContactController` injects `IRateLimitService`
- Test from different IPs (or clear in-memory cache by restarting app)

---

## 📞 Support

For additional help, refer to:
- `README_ADMIN_SETUP.md` - Detailed setup guide
- `ADMIN_IMPLEMENTATION_GUIDE.md` - Complete implementation reference
- GitHub Issues - Report bugs or request features

---

**Good luck with your deployment!** 🚀
