# Security Configuration Guide

## ✅ Secrets Removed from Repository

All sensitive keys have been removed from `appsettings.json` and are now managed through GitHub Secrets and environment variables.

## 🔐 GitHub Secrets Configuration

The following secrets are configured in your GitHub repository:
- **SMTP_PASSWORD** - Gmail app password for email notifications
- **GEMINI_API_KEY** - Google Gemini AI API key
- **RECAPTCHA_SECRET_KEY** - Google reCAPTCHA secret key

These secrets are automatically injected during deployment via GitHub Actions workflows.

## 📋 Updated Files

### 1. appsettings.json
- **SMTP Password**: Cleared (will use `SmtpSettings__Password` environment variable)
- **ReCaptcha SecretKey**: Cleared (will use `ReCaptcha__SecretKey` environment variable)
- **Gemini ApiKey**: Already empty (will use `Gemini__ApiKey` environment variable)

### 2. GitHub Workflows Updated
- `.github/workflows/main_mmsite2026.yml`
- `.github/workflows/main_marutimakwana.yml`

Both workflows now inject the secrets as environment variables during deployment:
```yaml
env:
  SmtpSettings__Password: ${{ secrets.SMTP_PASSWORD }}
  Gemini__ApiKey: ${{ secrets.GEMINI_API_KEY }}
  ReCaptcha__SecretKey: ${{ secrets.RECAPTCHA_SECRET_KEY }}
```

## 🔄 How It Works

ASP.NET Core automatically reads environment variables and maps them to configuration using the double underscore (`__`) notation:

- `SmtpSettings__Password` → `SmtpSettings:Password`
- `Gemini__ApiKey` → `Gemini:ApiKey`
- `ReCaptcha__SecretKey` → `ReCaptcha:SecretKey`

No code changes were needed in `Program.cs` - this is handled automatically by the configuration system.

## 🚀 Deployment

When you push to the main branch:
1. GitHub Actions builds the application
2. Secrets are injected as environment variables
3. Application is deployed to Azure with the secrets available at runtime

## 🛡️ Security Best Practices

✅ **Done:**
- Secrets removed from source code
- Secrets managed in GitHub repository settings
- Automatic injection during deployment

⚠️ **Remember:**
- Never commit `appsettings.json` with real secrets
- Rotate credentials if they were previously exposed
- Keep the ReCaptcha SiteKey in appsettings.json (it's public and safe to commit)

## 📝 Local Development

For local development, use **User Secrets**:

```bash
dotnet user-secrets set "SmtpSettings:Password" "your-password-here"
dotnet user-secrets set "Gemini:ApiKey" "your-api-key-here"
dotnet user-secrets set "ReCaptcha:SecretKey" "your-secret-key-here"
```

Or create a `appsettings.Development.json` file (ensure it's in `.gitignore`):

```json
{
  "SmtpSettings": {
    "Password": "your-local-password"
  },
  "Gemini": {
    "ApiKey": "your-local-api-key"
  },
  "ReCaptcha": {
    "SecretKey": "your-local-secret-key"
  }
}
```
