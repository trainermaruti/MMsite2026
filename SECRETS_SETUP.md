# Secrets Management Setup

## ✅ Development Environment (Current Setup)

The Gemini API key has been moved to **User Secrets** for secure local development.

### Current Configuration:
- **appsettings.json**: API key is empty (safe to commit to Git)
- **User Secrets**: API key is stored securely on your machine
- **Location**: `%APPDATA%\Microsoft\UserSecrets\a16ae9cf-3c85-428f-8e37-eb9956dae11b\secrets.json`

### Managing Secrets:

```bash
# View all secrets
dotnet user-secrets list

# Set a secret
dotnet user-secrets set "Gemini:ApiKey" "your-api-key-here"

# Remove a secret
dotnet user-secrets remove "Gemini:ApiKey"

# Clear all secrets
dotnet user-secrets clear
```

## 🚀 Testing the Website

### Run Locally:
```bash
dotnet run
```
or
```bash
dotnet watch run
```

The application will automatically load the API key from User Secrets during development.

## 🌐 Production Deployment

For production, you need to set the API key as an **environment variable** or use a secure configuration provider:

### Option 1: Environment Variable (Recommended)
```bash
# Windows (PowerShell)
$env:Gemini__ApiKey="your-production-api-key"

# Linux/macOS
export Gemini__ApiKey="your-production-api-key"
```

### Option 2: appsettings.Production.json
Create a file `appsettings.Production.json` (do NOT commit to Git):
```json
{
  "Gemini": {
    "ApiKey": "your-production-api-key"
  }
}
```

### Option 3: Azure App Service
In Azure Portal:
1. Go to your App Service
2. Navigate to **Configuration** → **Application settings**
3. Add new setting: `Gemini__ApiKey` = `your-production-api-key`

### Option 4: IIS/Windows Server
Set in web.config or environment variables in IIS Manager.

## 🔒 Security Best Practices

✅ **DO:**
- Use User Secrets for development
- Use environment variables for production
- Keep appsettings.json with empty/placeholder values
- Add `appsettings.Production.json` to `.gitignore`

❌ **DON'T:**
- Commit actual API keys to Git
- Share secrets in chat or documentation
- Use the same API key for dev and production

## 📝 Other Secrets to Secure

Consider securing these as well:
- `SmtpSettings:Password` - Email password
- `ConnectionStrings:DefaultConnection` - Database connection
- `ReCaptcha:SecretKey` - reCAPTCHA secret

### Set them all at once:
```bash
dotnet user-secrets set "SmtpSettings:Password" "your-email-password"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
dotnet user-secrets set "ReCaptcha:SecretKey" "your-recaptcha-secret"
```
