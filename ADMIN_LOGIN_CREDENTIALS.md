# Admin Portal Login Credentials

## Default Credentials (Development/Demo)

**Email:** `admin@marutitraining.com`  
**Password:** `Admin@123`

## Login URL

- **Local:** http://localhost:5000/Account/Login
- **Production:** https://marutimakwana.azurewebsites.net/Account/Login

## Admin Dashboard

After login, you'll be redirected to:
- https://marutimakwana.azurewebsites.net/Admin/Dashboard

## Diagnostic Endpoints

Check if admin user exists:
- https://marutimakwana.azurewebsites.net/admin/check-admin

Check database status:
- https://marutimakwana.azurewebsites.net/health

Force data import:
- https://marutimakwana.azurewebsites.net/admin/force-import-data

## For Production: Set Custom Credentials

### Option 1: Azure App Service Configuration

1. Go to Azure Portal → Your App Service → Configuration
2. Add Application Settings:
   - `Admin__Email` = your-email@example.com
   - `Admin__Password` = YourSecurePassword123!
3. Save and restart the app

### Option 2: User Secrets (Local Development)

```bash
dotnet user-secrets set "Admin:Email" "your-email@example.com"
dotnet user-secrets set "Admin:Password" "YourSecurePassword123!"
```

## Password Requirements

- At least 8 characters
- Contains uppercase letter
- Contains lowercase letter
- Contains digit
- Contains special character

## Troubleshooting

If you can't login:

1. Check if admin user exists: `/admin/check-admin`
2. Verify credentials are correct (case-sensitive)
3. Check if cookies are enabled
4. Try incognito/private browsing mode
5. Clear browser cache and cookies

## Security Notes

⚠️ **IMPORTANT:** The default credentials (`Admin@123`) are for development/demo only.  
🔒 **CHANGE THEM IMMEDIATELY** in production environments!
