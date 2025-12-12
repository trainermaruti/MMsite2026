# Admin Authentication Setup Guide

This document describes how to set up and configure the admin authentication system for the Maruti Training Portal.

## Overview

The admin area uses ASP.NET Core Identity with role-based authorization. Admin credentials are **never stored in source code** - they are managed via User Secrets (development) or Environment Variables (production).

---

## Development Setup (Local Machine)

### 1. Initialize User Secrets

User Secrets allow you to store sensitive data outside your project folder, preventing accidental commits to source control.

```powershell
# Navigate to project directory
cd c:\maruti-makwana

# Initialize user secrets (if not already done)
dotnet user-secrets init
```

### 2. Set Admin Credentials

```powershell
# Set admin email
dotnet user-secrets set "Admin:Email" "admin@marutitraining.com"

# Set admin password (must be 8+ characters with uppercase, lowercase, digit, and special character)
dotnet user-secrets set "Admin:Password" "YourSecurePassword123!"
```

**Password Requirements:**
- Minimum 8 characters
- At least one uppercase letter (A-Z)
- At least one lowercase letter (a-z)
- At least one digit (0-9)
- At least one non-alphanumeric character (!@#$%^&*)

### 3. Verify User Secrets

```powershell
# List all user secrets
dotnet user-secrets list
```

Expected output:
```
Admin:Email = admin@marutitraining.com
Admin:Password = YourSecurePassword123!
```

### 4. Run the Application

```powershell
dotnet run
```

On first startup, the application will:
1. Read credentials from User Secrets
2. Create the "Admin" role (if it doesn't exist)
3. Create the admin user account (if it doesn't exist)
4. Assign the Admin role to the user

---

## Production Setup (Azure App Service / Docker)

### Option A: Azure App Service

1. **Navigate to your Azure App Service** in the Azure Portal
2. Go to **Settings → Configuration**
3. Under **Application settings**, add the following:

| Name | Value | Example |
|------|-------|---------|
| `ASPNETCORE_Admin__Email` | Your admin email | `admin@marutitraining.com` |
| `ASPNETCORE_Admin__Password` | Your admin password | `SecurePassword123!` |
| `ConnectionStrings__DefaultConnection` | SQL Server connection string | `Server=tcp:yourserver.database.windows.net,1433;...` |

4. Click **Save** and restart the app service

### Option B: Docker Compose

Create a `.env` file in your deployment directory (do NOT commit this file):

```env
ASPNETCORE_Admin__Email=admin@marutitraining.com
ASPNETCORE_Admin__Password=SecurePassword123!
ConnectionStrings__DefaultConnection=Server=db;Database=MarutiTraining;User=sa;Password=YourDbPassword123!;
```

Update `docker-compose.yml`:

```yaml
version: '3.8'
services:
  web:
    image: marutitraining:latest
    environment:
      - ASPNETCORE_Admin__Email=${ASPNETCORE_Admin__Email}
      - ASPNETCORE_Admin__Password=${ASPNETCORE_Admin__Password}
      - ConnectionStrings__DefaultConnection=${ConnectionStrings__DefaultConnection}
    ports:
      - "5000:80"
```

### Option C: GitHub Actions (CI/CD)

Add the following secrets to your GitHub repository:

1. Go to **Settings → Secrets and variables → Actions**
2. Add these repository secrets:

- `ADMIN_EMAIL`: Admin email address
- `ADMIN_PASSWORD`: Admin password
- `AZURE_PUBLISH_PROFILE`: Download from Azure App Service (optional, for Azure deployment)

The CI/CD workflow will inject these as environment variables during deployment.

---

## How Admin Seeding Works

The `AdminSeeder.cs` class runs automatically during application startup:

```csharp
// In Program.cs
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await AdminSeeder.SeedAdminUserAsync(services);
}
```

**Seeding Logic:**
1. Reads `Admin:Email` and `Admin:Password` from configuration (User Secrets or environment variables)
2. Creates "Admin" role if it doesn't exist
3. Creates admin user if it doesn't exist
4. Sets `EmailConfirmed = true` (bypasses email verification for admin)
5. Assigns "Admin" role to the user

**Security Notes:**
- Passwords are hashed using ASP.NET Core Identity's PBKDF2 algorithm
- Admin seeding runs on every startup but is **idempotent** (safe to run multiple times)
- If admin user already exists, seeding does nothing

---

## Accessing the Admin Area

### Login URL

```
https://yourdomain.com/Admin/Account/Login
```

Or navigate directly to any admin-protected page (you'll be redirected to login).

### Admin Features

Once logged in, you can access:

- **Dashboard**: `/Admin/Dashboard` - Statistics and charts
- **Manage Trainings**: `/Admin/Trainings` (via sidebar)
- **Manage Courses**: `/Admin/Courses`
- **Manage Events**: `/Admin/Events`
- **View Contact Messages**: `/Admin/Contact`
- **Edit Profile**: `/Admin/Profile`

### Lockout Policy

After **5 failed login attempts**, the account is locked for **15 minutes** to prevent brute-force attacks.

---

## Changing Admin Credentials

### Development

```powershell
# Update password
dotnet user-secrets set "Admin:Password" "NewSecurePassword456!"

# Restart the application
dotnet run
```

**Note:** You must manually update the password in the database or delete the admin user to force re-seeding.

To delete and re-seed:
```powershell
# Delete the database
Remove-Item MarutiTrainingPortal.db

# Run app (will recreate database and seed admin)
dotnet run
```

### Production

1. Update the environment variable in Azure App Service / Docker
2. Delete the existing admin user from the database
3. Restart the application (will re-seed with new password)

Alternatively, use Identity's password reset functionality (requires implementing email sending).

---

## Database Connection Strings

### Development (SQLite)

Default configuration in `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=MarutiTrainingPortal.db"
  }
}
```

### Production (SQL Server)

Set via environment variable:

```
ConnectionStrings__DefaultConnection=Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=MarutiTraining;Persist Security Info=False;User ID=yourusername;Password=yourpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

Or in `appsettings.Production.json` using environment variable substitution:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "${ConnectionStrings__DefaultConnection}"
  }
}
```

---

## Troubleshooting

### "Admin user not created on startup"

**Check logs for errors:**
```powershell
dotnet run --verbosity detailed
```

**Common causes:**
- User Secrets not configured: Run `dotnet user-secrets list` to verify
- Password doesn't meet complexity requirements
- Database connection failed

### "Invalid login attempt"

**Possible causes:**
- Incorrect email/password
- Admin user not created (check database `AspNetUsers` table)
- Admin role not assigned (check `AspNetUserRoles` table)
- Account locked after 5 failed attempts (wait 15 minutes)

**To unlock manually:**
```sql
UPDATE AspNetUsers 
SET LockoutEnd = NULL, AccessFailedCount = 0 
WHERE Email = 'admin@marutitraining.com';
```

### "Environment variables not loading in production"

**Azure App Service:**
- Restart the app after setting environment variables
- Check **Configuration → Application settings** shows your variables
- Use **Advanced edit** to verify JSON structure

**Docker:**
- Ensure `.env` file is in the same directory as `docker-compose.yml`
- Check environment variables are passed correctly: `docker compose config`
- Restart containers: `docker compose down && docker compose up -d`

---

## Security Best Practices

1. **Never commit credentials to source control**
   - Add `appsettings.Production.json` to `.gitignore` if it contains sensitive data
   - Use User Secrets (dev) and Environment Variables (prod)

2. **Use strong passwords**
   - Minimum 16 characters for production
   - Use a password manager to generate random passwords

3. **Enable HTTPS in production**
   - Configure SSL certificates in Azure App Service
   - Redirect HTTP to HTTPS in `Program.cs`

4. **Implement password rotation**
   - Change admin password every 90 days
   - Consider implementing multi-factor authentication (MFA)

5. **Monitor failed login attempts**
   - Check application logs for suspicious activity
   - Consider integrating with Azure Application Insights

6. **Limit admin access**
   - Only create admin accounts for trusted users
   - Consider implementing audit logging for admin actions

---

## Additional Resources

- [ASP.NET Core Identity Documentation](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [Safe Storage of App Secrets in Development](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Azure App Service Application Settings](https://learn.microsoft.com/en-us/azure/app-service/configure-common)

---

## Support

For issues or questions, contact the development team or refer to the main [README.md](README.md).
