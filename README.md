# Maruti Makwana Training Portal

A professional ASP.NET Core 8 MVC website to showcase corporate training expertise, video-based learning courses, upcoming events, and direct contact options.

## Project Overview

This is a comprehensive web application built with:
- **.NET 8 LTS** - Latest long-term support version
- **ASP.NET Core MVC** - Model-View-Controller architecture
- **Entity Framework Core** - Object-relational mapping
- **SQL Server LocalDB** - Local database for development
- **Bootstrap 5** - Responsive UI framework
- **Font Awesome Icons** - Professional iconography

## Features

### 1. **Home Page**
- Hero section with call-to-action buttons
- Quick statistics display (trainings, students, courses, companies)
- Featured service cards
- About section preview
- Contact CTA (Call-To-Action)

### 2. **Past Trainings**
- Display all delivered corporate trainings
- Show training details (company, date, participants, topics)
- Create, edit, and delete training records
- Training images/thumbnails
- Full CRUD operations

### 3. **Video Courses**
- Showcase recorded learning courses
- Course categories and difficulty levels
- Course metrics (enrollment count, ratings, duration, price)
- Self-paced learning platform
- Video course management

### 4. **Events & Calendar**
- Schedule upcoming training events and webinars
- Event registration management
- Event capacity tracking
- Location and timing information
- Event type classification

### 5. **Profile / About Me**
- Professional profile information
- Expertise and skills showcase
- Certifications and achievements
- Professional statistics
- Social media links (LinkedIn, Twitter, GitHub)
- Direct contact information

### 6. **Contact Form**
- Email contact form
- Direct email and WhatsApp contact options
- Contact message storage in database
- Confirmation messages
- Quick response communication

## Project Structure

```
MarutiTrainingPortal/
├── Models/                          # Data models
│   ├── Training.cs                  # Training history
│   ├── Course.cs                    # Video course information
│   ├── TrainingEvent.cs             # Upcoming events
│   ├── Profile.cs                   # Trainer profile
│   └── ContactMessage.cs            # Contact form submissions
├── Controllers/                     # MVC Controllers
│   ├── HomeController.cs
│   ├── TrainingsController.cs
│   ├── CoursesController.cs
│   ├── EventsController.cs
│   ├── ProfileController.cs
│   └── ContactController.cs
├── Views/                           # Razor Views
│   ├── Home/
│   │   └── Index.cshtml
│   ├── Trainings/
│   │   └── Index.cshtml
│   ├── Courses/
│   │   └── Index.cshtml
│   ├── Events/
│   │   └── Index.cshtml
│   ├── Profile/
│   │   └── About.cshtml
│   ├── Contact/
│   │   └── Index.cshtml
│   └── Shared/
│       ├── _Layout.cshtml           # Master layout
│       └── _ValidationScriptsPartial.cshtml
├── Data/
│   └── ApplicationDbContext.cs      # EF Core DbContext
├── Migrations/                      # Database migrations
├── wwwroot/                         # Static files (CSS, JS, images)
├── Program.cs                       # Application configuration
├── appsettings.json                 # Configuration settings
└── MarutiTrainingPortal.csproj      # Project file

```

## Database Schema

### Tables

1. **Trainings** - Corporate training history
   - Id, Title, Description, Company, DeliveryDate, ParticipantsCount, Topics, ImageUrl

2. **Courses** - Video courses
   - Id, Title, Description, Category, ThumbnailUrl, VideoUrl, DurationMinutes, Level, Price, TotalEnrollments, Rating, PublishedDate

3. **TrainingEvents** - Upcoming events
   - Id, Title, Description, StartDate, EndDate, Location, EventType, MaxParticipants, RegisteredParticipants, RegistrationLink, ImageUrl

4. **Profiles** - Trainer profile information
   - Id, FullName, Title, Bio, ProfileImageUrl, Email, PhoneNumber, WhatsAppNumber, Expertise, TotalTrainingsDone, TotalStudents, LinkedInUrl, TwitterUrl, GitHubUrl, CertificationsAndAchievements

5. **ContactMessages** - Contact form submissions
   - Id, Name, Email, PhoneNumber, Subject, Message, IsRead, CreatedDate

## Getting Started

### Prerequisites
- .NET 8 SDK or later
- SQL Server (LocalDB included with Visual Studio)
- Visual Studio 2022 or VS Code with C# extension

### Installation & Setup

1. **Clone or open the project in Visual Studio Code**

2. **Restore NuGet packages:**
   ```powershell
   dotnet restore
   ```

3. **Create and apply database migration:**
   ```powershell
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. **Run the application:**
   ```powershell
   dotnet run
   ```

5. **Access the website:**
   - Open browser and go to `https://localhost:5001` or `http://localhost:5000`

## Configuration

### Database Connection String
Edit `appsettings.json` to change the database connection:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MarutiTrainingPortalDb;Trusted_Connection=true;"
  }
}
```

### Profile Information
The default profile is seeded during migration. Edit it through the "Edit Profile" page or update it in the database directly.

## Customization Guide

### 1. Update Personal Information
- Navigate to `/Profile/About` page
- Click "Edit Profile" to update:
  - Full name, title, bio
  - Profile image URL
  - Contact information
  - Social media links
  - Certifications and achievements

### 2. Add Training Records
- Go to `/Trainings` and click "Add New Training"
- Fill in:
  - Training title and description
  - Company name
  - Delivery date
  - Number of participants
  - Topics covered
  - Training image URL

### 3. Add Video Courses
- Visit `/Courses` and click "Add New Course"
- Provide:
  - Course title and description
  - Category and difficulty level
  - Thumbnail and video URLs
  - Duration and price
  - Enrollment and rating information

### 4. Create Events
- Go to `/Events` and click "Add New Event"
- Enter:
  - Event title and description
  - Start and end dates
  - Location details
  - Maximum capacity
  - Registration link

### 5. Replace Image URLs
- Update images by replacing URLs in respective sections
- Host images on cloud storage (Azure Blob Storage, AWS S3, or similar)
- Update URLs in the database records

## Adding Features

### Add Admin Panel
To add admin functionality:
1. Create an AdminController
2. Implement authentication (ASP.NET Identity or similar)
3. Add admin views for managing content

### Add Payment Integration
For course purchases:
1. Integrate Razorpay, PayPal, or Stripe
2. Add payment models
3. Create payment processing logic

### Add Email Notifications
1. Configure SMTP settings
2. Add email service
3. Send confirmations for bookings and messages

### Add Azure Integration
1. Store images in Azure Blob Storage
2. Use Azure App Service for hosting
3. Implement Azure SQL Database for production

## Database Seed Data

Default profile is automatically created with migration. Update the following in `Data/ApplicationDbContext.cs`:
```csharp
new Profile
{
    FullName = "Maruti Makwana",
    Title = "Corporate Trainer - Azure Cloud & AI",
    Email = "maruti@example.com",
    PhoneNumber = "+91-XXXXXXXXXX",
    WhatsAppNumber = "+91-XXXXXXXXXX",
    // ... other properties
}
```

## Deployment

### Deploy to Azure App Service
1. Publish to Azure:
   ```powershell
   dotnet publish -c Release
   ```
2. Create App Service in Azure Portal
3. Deploy using Visual Studio or Azure CLI

### Deploy to IIS
1. Publish as self-contained:
   ```powershell
   dotnet publish -c Release -r win-x64 --self-contained
   ```
2. Copy files to IIS server
3. Configure IIS application pool

## Technology Stack

| Component | Technology |
|-----------|-----------|
| **Runtime** | .NET 8 LTS |
| **Framework** | ASP.NET Core MVC |
| **Database** | SQL Server LocalDB |
| **ORM** | Entity Framework Core 8.0.11 |
| **Frontend** | Bootstrap 5, Font Awesome |
| **UI Components** | Razor Views (.cshtml) |

## NuGet Packages Used

- `Microsoft.EntityFrameworkCore` - 8.0.11
- `Microsoft.EntityFrameworkCore.SqlServer` - 8.0.11
- `Microsoft.EntityFrameworkCore.Tools` - 8.0.11

## Troubleshooting

### Migration Issues
```powershell
# Remove last migration if needed
dotnet ef migrations remove

# View migration status
dotnet ef migrations list
```

### Database Connection Issues
- Verify SQL Server LocalDB is running
- Check connection string in `appsettings.json`
- Ensure database name is correct

### Port Already in Use
```powershell
# Run on different port
dotnet run --urls "https://localhost:5002"
```

## Future Enhancements

- [ ] Admin dashboard for content management
- [ ] User authentication and roles
- [ ] Course purchase and payment integration
- [ ] Student enrollment tracking
- [ ] Email notifications
- [ ] Search and filtering
- [ ] Mobile app
- [ ] Analytics and reporting
- [ ] Certificate generation
- [ ] Discussion forums

## Support & Contact

For questions or support regarding this application:
- Email: maruti@example.com
- WhatsApp: +91-XXXXXXXXXX
- LinkedIn: https://linkedin.com/in/maruti

## License

This project is personal property of Maruti Makwana. All rights reserved.

## Notes

- Update all placeholder information (email, phone, URLs, social links)
- Replace placeholder images with actual profile and training images
- Configure actual database connection for production
- Implement proper security measures for production deployment
- Add SSL/TLS certificates for HTTPS
- Implement backup and recovery procedures

---

**Created:** November 28, 2025
**Framework Version:** ASP.NET Core 8 LTS
**Last Updated:** November 28, 2025
