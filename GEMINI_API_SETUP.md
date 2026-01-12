# API Keys and Secrets Setup Guide

## Security Notice
**NEVER** commit API keys or secrets to version control. All sensitive configuration should be stored as environment variables:
- `GEMINI_API` - Gemini AI API key
- `RECAPTCHA_SITEKEY` - ReCaptcha site key  
- `RECAPTCHA_SECRET` - ReCaptcha secret key

---


## Setup Instructions

### For Local Development (Windows PowerShell)

**Option 1: Set for current session only**
```powershell
$env:GEMINI_API = "YOUR_GEMINI_API_KEY"
$env:RECAPTCHA_SITEKEY = "YOUR_RECAPTCHA_SITE_KEY"
$env:RECAPTCHA_SECRET = "YOUR_RECAPTCHA_SECRET_KEY"
```

**Option 2: Set permanently (User level - RECOMMENDED)**
```powershell
[System.Environment]::SetEnvironmentVariable('GEMINI_API', 'YOUR_GEMINI_API_KEY', 'User')
[System.Environment]::SetEnvironmentVariable('RECAPTCHA_SITEKEY', 'YOUR_RECAPTCHA_SITE_KEY', 'User')
[System.Environment]::SetEnvironmentVariable('RECAPTCHA_SECRET', 'YOUR_RECAPTCHA_SECRET_KEY', 'User')
```

**Option 3: Set permanently (System level - requires admin)**
```powershell
[System.Environment]::SetEnvironmentVariable('GEMINI_API', 'YOUR_GEMINI_API_KEY', 'Machine')
[System.Environment]::SetEnvironmentVariable('RECAPTCHA_SITEKEY', 'YOUR_RECAPTCHA_SITE_KEY', 'Machine')
[System.Environment]::SetEnvironmentVariable('RECAPTCHA_SECRET', 'YOUR_RECAPTCHA_SECRET_KEY', 'Machine')
```

After setting permanently, **restart your terminal/IDE** for changes to take effect.

---


### For Azure App Service Deployment

1. Go to Azure Portal → Your App Service
2. Navigate to **Settings** → **Configuration**
3. Under **Application settings**, click **+ New application setting**
4. Add all three secrets:
   - **Name**: `GEMINI_API` | **Value**: Your Gemini API key
   - **Name**: `RECAPTCHA_SITEKEY` | **Value**: Your ReCaptcha site key
   - **Name**: `RECAPTCHA_SECRET` | **Value**: Your ReCaptcha secret key
5. Click **OK** for each, then **Save**
6. **Restart** your App Service

---

### For Docker Deployment

**In docker-compose.yml:**
```yaml
services:
  webapp:
    environment:
      - GEMINI_API=${GEMINI_API}
```

**Or pass directly:**
```bash
docker run -e GEMINI_API="YOUR_API_KEY_HERE" your-image
```

---

### For Other Cloud Providers

**AWS Elastic Beanstalk:**
- Configuration → Software → Environment properties
- Add: `GEMINI_API` = your key

**Google Cloud Run:**
```bash
gcloud run services update SERVICE_NAME \
  --set-env-vars GEMINI_API="YOUR_API_KEY_HERE"
```

**Heroku:**
```bash
heroku config:set GEMINI_API="YOUR_API_KEY_HERE"
```

---

## Verification

To verify the environment variables are set:

**PowerShell:**
```powershell
$env:GEMINI_API
$env:RECAPTCHA_SITEKEY
$env:RECAPTCHA_SECRET
```

**CMD:**
```cmd
echo %GEMINI_API%
echo %RECAPTCHA_SITEKEY%
echo %RECAPTCHA_SECRET%
```

**Linux/Mac:**
```bash
echo $GEMINI_API
echo $RECAPTCHA_SITEKEY
echo $RECAPTCHA_SECRET
```

---

## How It Works

The application checks for the API key in this order:
1. **Environment variable** `GEMINI_API` (preferred for production)
2. **appsettings.json** `Gemini:ApiKey` (fallback for development)

Code implementation in `GeminiService.cs`:
```csharp
var apiKey = Environment.GetEnvironmentVariable("GEMINI_API") ?? _configuration["Gemini:ApiKey"];
```

---

## Security Best Practices

✅ **DO:**
- Use environment variables for production
- Store keys in Azure Key Vault or similar secret management systems
- Keep `appsettings.json` with empty ApiKey in source control
- Use `appsettings.Development.json` for local development (gitignored)

❌ **DON'T:**
- Hardcode API keys in source code
- Commit API keys to Git repositories
- Share API keys in team chats or emails
- Use production keys in development environments

---

## Troubleshooting

**Bot shows "I'm currently not available" error:**
- Verify environment variable is set: `$env:GEMINI_API`
- Restart your application after setting the variable
- Check Azure App Service configuration if deployed
- Verify the API key is valid at https://aistudio.google.com/app/apikey

**Environment variable not working:**
- Restart your terminal/IDE after setting
- Check spelling: `GEMINI_API` (case-sensitive on Linux/Mac)
- For system-level changes, you may need to restart Windows
