# Admin Login Guide

## Admin Credentials

- **Email:** admin@marutitraining.com
- **Password:** Admin@123456

## How to Access Admin Features

### 1. Login as Admin
1. Navigate to http://localhost:5204
2. Click on **"Admin Login"** in the top-right corner of the navigation bar
3. Enter the admin credentials:
   - Email: `admin@marutitraining.com`
   - Password: `Admin@123456`
4. Click "Login"

### 2. Admin Features
Once logged in as admin, you will see:
- **"Logout"** button in the navigation bar
- **"Admin"** link in the navigation bar (goes to admin dashboard)

### 3. Managing Content (Admin Only)
As an admin, you can:
- **Trainings:** Add New, Edit, Delete training records
- **Courses:** Add New, Edit, Delete courses
- **Events:** Add New, Edit, Delete events

These buttons are **ONLY visible to logged-in admin users**. Regular visitors cannot see or use these management functions.

### 4. Testing Authorization
To verify the authorization is working:
1. Visit the site without logging in → Management buttons should be **hidden**
2. Login as admin → Management buttons should be **visible**
3. Logout → Management buttons should be **hidden again**

## Security Notes
- All Create, Edit, Delete operations are protected by `[Authorize(Roles = "Admin")]`
- Management buttons are conditionally rendered only for admin users
- Non-admin users cannot access admin-only pages even if they know the URL
- Change the default password in production!

## Changing Admin Credentials
Edit `appsettings.Development.json` (for development) or use User Secrets (recommended for production):
```json
{
  "Admin": {
    "Email": "your-email@example.com",
    "Password": "YourSecurePassword123!"
  }
}
```
