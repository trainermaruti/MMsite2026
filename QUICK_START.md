# Maruti Makwana Training Portal - Quick Start Guide

Welcome! This guide will help you get your professional training website up and running.

## 🎯 Project Summary

Your **Maruti Makwana Training Portal** is a modern, professional website built with:
- **ASP.NET Core 8 MVC** - Industry-standard web framework
- **SQL Server LocalDB** - Local database for development
- **Bootstrap 5** - Professional, responsive design
- **Entity Framework Core** - Database management

## ✅ What's Included

### 📁 Complete Project Structure
- ✓ 5 Data Models (Training, Course, Event, Profile, Contact)
- ✓ 5 Controllers with full CRUD operations
- ✓ Responsive Razor Views (.cshtml)
- ✓ Professional navigation and footer
- ✓ Database with seed data

### 🎨 Features Pre-Built
1. **Home Page** - Hero section with stats and CTA
2. **Past Trainings** - Showcase your training history
3. **Video Courses** - Display recorded learning material
4. **Events Calendar** - Manage upcoming trainings
5. **Profile/About** - Professional bio and achievements
6. **Contact Form** - Direct messaging and contact info

### 📊 Database
- SQL Server LocalDB (already created and migrated)
- Pre-populated with your profile information
- Tables: Trainings, Courses, TrainingEvents, Profiles, ContactMessages

## 🚀 Getting Started (3 Simple Steps)

### Step 1: Open and Run
```powershell
# Open the project folder in VS Code (already done)
# Open terminal and run:
dotnet run
```

### Step 2: Access Website
```
Open browser: https://localhost:5001
or            http://localhost:5000
```

### Step 3: Start Customizing
- Visit `/Profile/About` to edit your profile
- Add trainings at `/Trainings` → "Add New Training"
- Upload courses at `/Courses` → "Add New Course"
- Schedule events at `/Events` → "Add New Event"

## 📝 Customization Checklist

### Immediate Customization (Next 10 minutes)

- [ ] **Update Personal Details**
  - Go to `/Profile/About` → Click "Edit Profile"
  - Update: Full name, Bio, Email, Phone, WhatsApp
  - Add profile image URL

- [ ] **Update Contact Information**
  - Replace email: `maruti@example.com` → your email
  - Replace phone: `+91-XXXXXXXXXX` → your actual numbers
  - Update social media links (LinkedIn, Twitter, GitHub)

- [ ] **Add Profile Image**
  - Upload to: Azure Blob Storage, AWS S3, or Imgur
  - Copy image URL to profile image field
  - Set URL: `/images/profile-picture.jpg` or cloud URL

### Short-term Customization (Next Hour)

- [ ] **Add Past Trainings**
  - Go to `/Trainings` 
  - Click "Add New Training"
  - Add 3-5 representative training records
  - Include: Company, Date, Participants, Topics

- [ ] **Add Video Courses**
  - Visit `/Courses`
  - Click "Add New Course"
  - Add your recorded course information
  - Provide YouTube/Vimeo video links

- [ ] **Schedule Events**
  - Go to `/Events`
  - Click "Add New Event"
  - Add upcoming webinars or workshops
  - Set registration link

### Medium-term Customization (Next Day)

- [ ] **Replace Image URLs**
  - Use cloud storage for all images
  - Update training thumbnails
  - Add course preview images
  - Add event banners

- [ ] **Update Home Page Stats**
  - Edit in `/Views/Home/Index.cshtml`
  - Update: "50+", "2000+", "25+", "30+" with real numbers

- [ ] **Configure Email**
  - Set up SMTP for contact form responses
  - Or use Azure SendGrid

## 🔧 Key Pages URL Structure

| Feature | URL | Action |
|---------|-----|--------|
| Home | `/` | View dashboard |
| Trainings List | `/Trainings` | List all trainings |
| Add Training | `/Trainings/Create` | Add new training |
| Edit Training | `/Trainings/Edit/1` | Modify training |
| Courses List | `/Courses` | View courses |
| Add Course | `/Courses/Create` | Upload course |
| Events | `/Events` | View calendar |
| Add Event | `/Events/Create` | Schedule event |
| Profile | `/Profile/About` | View full profile |
| Edit Profile | `/Profile/Edit` | Update profile |
| Contact | `/Contact` | Contact form |

## 💾 Database Management

### View Database in SQL Server Management Studio
1. Open SQL Server Management Studio
2. Connect to: `(localdb)\mssqllocaldb`
3. Database: `MarutiTrainingPortalDb`

### Create New Migration (if you modify models)
```powershell
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Reset Database
```powershell
dotnet ef database drop
dotnet ef database update
```

## 🎨 Styling & Design

### Current Color Scheme
- **Primary Purple**: `#667eea` to `#764ba2`
- Uses Bootstrap 5 classes
- Font Awesome icons for visual enhancement
- Responsive mobile-first design

### Customizing Colors
Edit `/Views/Shared/_Layout.cshtml` and component views to change colors throughout the site.

## 📦 What's Installed

### NuGet Packages
- `Microsoft.EntityFrameworkCore` - Database ORM
- `Microsoft.EntityFrameworkCore.SqlServer` - SQL Server adapter
- `Microsoft.EntityFrameworkCore.Tools` - Database tools

### Frontend Libraries (via CDN)
- Bootstrap 5 - UI framework
- Font Awesome 6.4 - Icons
- jQuery - JavaScript utilities

## 🌐 Deployment When Ready

### Option 1: Azure App Service
```powershell
# Publish
dotnet publish -c Release

# Deploy to Azure
# Use Azure Portal or VS Code Azure extension
```

### Option 2: IIS (Windows Server)
```powershell
# Publish as standalone
dotnet publish -c Release -r win-x64 --self-contained
```

## ❓ Troubleshooting

### Port Already in Use
```powershell
dotnet run --urls "https://localhost:5002"
```

### Database Connection Error
1. Ensure SQL LocalDB is installed
2. Check connection string in `appsettings.json`
3. Run: `dotnet ef database update`

### Build Errors
```powershell
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

## 📞 Next Steps

1. **Add Content** - Fill in your training, courses, events
2. **Customize Design** - Update colors and layout as needed
3. **Add Images** - Replace placeholder URLs with real images
4. **Test Features** - Verify all pages and forms work
5. **Deploy** - Move to Azure or web host when ready

## 🔐 Production Checklist

- [ ] Update all placeholder contact information
- [ ] Replace all placeholder image URLs
- [ ] Configure SSL/TLS certificate
- [ ] Set up database backups
- [ ] Configure email/SMTP for contact form
- [ ] Test all forms and submissions
- [ ] Optimize images for web
- [ ] Set up monitoring and logging
- [ ] Review security settings
- [ ] Plan maintenance schedule

## 📚 Resources

- [ASP.NET Core Documentation](https://learn.microsoft.com/aspnet/core)
- [Entity Framework Core](https://learn.microsoft.com/ef/core)
- [Bootstrap 5 Documentation](https://getbootstrap.com/docs/5.0)
- [Razor View Engine](https://learn.microsoft.com/aspnet/core/mvc/views/razor)

## 🎓 Learning Tips

1. **Models** - Understand how data flows through models
2. **Controllers** - Learn CRUD operations in controllers
3. **Views** - Customize Razor views for your needs
4. **Migrations** - Use EF Core to manage database changes

## 📧 Support

For help with specific features:
- Check the main README.md for detailed documentation
- Review existing code comments in controllers
- Test features locally before deploying

---

## 🎉 Ready to Launch?

Your website is ready! 

1. Start by running: `dotnet run`
2. Visit: `https://localhost:5001`
3. Begin adding your content
4. Customize styling as needed
5. Deploy when satisfied

**Happy training! Good luck with your portal! 🚀**

---

*Last Updated: November 28, 2025*
*Framework: ASP.NET Core 8 LTS*
