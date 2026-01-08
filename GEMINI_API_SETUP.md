# Gemini API Key Setup Guide

## Security Notice
⚠️ **The Gemini API key is now secured using .NET User Secrets**

Your API key is stored securely outside the project directory and will NOT be committed to Git.

## For Developers Setting Up Locally

### Main Project (MarutiTrainingPortal)
```powershell
cd c:\Users\Skill\Desktop\MMsite2026-1
dotnet user-secrets set "Gemini:ApiKey" "YOUR_API_KEY_HERE"
```

### SkilltechBot Project
```powershell
cd c:\Users\Skill\Desktop\MMsite2026-1\skilltechBot
dotnet user-secrets set "Gemini:ApiKey" "YOUR_API_KEY_HERE"
```

## Verification

To verify your secrets are set correctly:

```powershell
# For main project
cd c:\Users\Skill\Desktop\MMsite2026-1
dotnet user-secrets list

# For skilltechBot
cd c:\Users\Skill\Desktop\MMsite2026-1\skilltechBot
dotnet user-secrets list
```

You should see:
```
Gemini:ApiKey = YOUR_API_KEY_HERE
```

## For Production Deployment

For Azure deployment, use Azure App Service Configuration:

1. Go to Azure Portal → Your App Service
2. Navigate to **Configuration** → **Application settings**
3. Add new setting:
   - **Name**: `Gemini:ApiKey`
   - **Value**: Your actual Gemini API key
4. Save changes and restart the app

## How It Works

- In **development**: The app reads the API key from User Secrets
- In **production**: The app reads from Azure App Service Configuration or environment variables
- User Secrets are stored at: `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`
- This file is NOT part of your project and will never be committed to Git

## Getting a Gemini API Key

1. Visit [Google AI Studio](https://aistudio.google.com/apikey)
2. Sign in with your Google account
3. Click "Get API Key" or "Create API Key"
4. Copy the generated key
5. Store it using the commands above

## Good Security Practices

✅ **DO:**
- Use User Secrets for local development
- Use Azure App Service Configuration for production
- Keep your API key private
- Rotate your API key periodically

❌ **DON'T:**
- Commit API keys in appsettings.json
- Share your API key in chat, email, or documentation
- Push API keys to Git repositories
- Store API keys in source code comments

---

**Status**: ✅ Your API key is now secure!
**Last Updated**: January 8, 2026
