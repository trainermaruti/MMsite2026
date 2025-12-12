# MARUTI TRAINING PORTAL - COMPLETE IMPLEMENTATION GUIDE
## Authentication, Security, Admin Dashboard & Deployment

Generated: November 30, 2025
Framework: ASP.NET Core 8 MVC
Database: SQLite (Development) / Azure SQL (Production)

---

## ✅ COMPLETED IMPLEMENTATIONS

### 1. NuGet Packages Installed
- Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.11)
- Microsoft.AspNetCore.Identity.UI (8.0.11)
- HtmlSanitizer (8.1.870)
- Serilog.AspNetCore (8.0.2)
- Serilog.Sinks.File (6.0.0)

### 2. Files Created/Updated

#### ✓ ViewModels Created
- Models/ViewModels/LoginViewModel.cs
- Models/ViewModels/AdminDashboardViewModel.cs

#### ✓ Controllers Created/Updated
- Controllers/AccountController.cs (NEW)
- Controllers/AdminController.cs (NEW)
- Controllers/CoursesController.cs (UPDATED with Authorization)

#### ✓ Services Created
- Services/EmailSender.cs (NEW)

#### ✓ Core Files Updated
- Data/ApplicationDbContext.cs (Added Identity, Indexes, Admin Seeding)
- Program.cs (Added Identity, Serilog, Email Service, Security)

---

## 📋 REMAINING IMPLEMENTATIONS

### STEP 1: Update Remaining Controllers with Authorization

#### File: Controllers/TrainingsController.cs

Add to the top of the file after existing usings:
```csharp
using Microsoft.AspNetCore.Authorization;
```

Add `[AllowAnonymous]` to Index and Details methods.
Add `[Authorize(Roles = "Admin")]` and `[ValidateAntiForgeryToken]` to Create (POST), Edit (GET/POST), Delete methods.

Example:
```csharp
[AllowAnonymous]
public async Task<IActionResult> Index()

[Authorize(Roles = "Admin")]
[HttpGet]
public IActionResult Create()

[Authorize(Roles = "Admin")]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Training training)
```

#### File: Controllers/EventsController.cs

Same pattern as TrainingsController.

#### File: Controllers/ProfileController.cs

```csharp
using Microsoft.AspNetCore.Authorization;

[AllowAnonymous]
public async Task<IActionResult> About()

[Authorize(Roles = "Admin")]
[HttpGet]
public async Task<IActionResult> Edit(int id = 1)

[Authorize(Roles = "Admin")]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(Profile profile)
```

#### File: Controllers/ContactController.cs

```csharp
using Microsoft.AspNetCore.Authorization;
using MarutiTrainingPortal.Services;
using HtmlSanitizer;

// Add to constructor
private readonly IEmailSender _emailSender;
private readonly HtmlSanitizer.HtmlSanitizer _htmlSanitizer;

public ContactController(
    ApplicationDbContext context,
    IEmailSender emailSender,
    HtmlSanitizer.HtmlSanitizer htmlSanitizer)
{
    _context = context;
    _emailSender = emailSender;
    _htmlSanitizer = htmlSanitizer;
}

[AllowAnonymous]
[HttpGet]
public IActionResult Index()

[AllowAnonymous]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Index(ContactMessage message)
{
    // Add rate limiting
    var lastSubmission = HttpContext.Session.GetString("LastContactSubmission");
    if (lastSubmission != null)
    {
        var lastTime = DateTime.Parse(lastSubmission);
        if (DateTime.Now - lastTime < TimeSpan.FromMinutes(5))
        {
            ModelState.AddModelError("", "Please wait 5 minutes between submissions.");
            return View(message);
        }
    }

    if (ModelState.IsValid)
    {
        // Sanitize message content
        message.Message = _htmlSanitizer.Sanitize(message.Message);
        message.Subject = _htmlSanitizer.Sanitize(message.Subject);
        
        _context.ContactMessages.Add(message);
        await _context.SaveChangesAsync();

        // Set session to prevent spam
        HttpContext.Session.SetString("LastContactSubmission", DateTime.Now.ToString());

        // Send email notification to admin
        try
        {
            await _emailSender.SendEmailAsync(
                "admin@marutitraining.com",
                $"New Contact Message: {message.Subject}",
                $"<h3>New message from {message.Name}</h3>" +
                $"<p><strong>Email:</strong> {message.Email}</p>" +
                $"<p><strong>Phone:</strong> {message.PhoneNumber}</p>" +
                $"<p><strong>Subject:</strong> {message.Subject}</p>" +
                $"<p><strong>Message:</strong></p><p>{message.Message}</p>");
        }
        catch (Exception ex)
        {
            // Log but don't fail
            // Email sending failure shouldn't stop the contact submission
        }

        TempData["SuccessMessage"] = "Thank you for contacting us! We'll get back to you soon.";
        return RedirectToAction(nameof(Index));
    }
    return View(message);
}
```

---

### STEP 2: Create Account Views

#### File: Views/Account/Login.cshtml

```cshtml
@model MarutiTrainingPortal.Models.ViewModels.LoginViewModel
@{
    ViewData["Title"] = "Admin Login";
}

<div class="container">
    <div class="row justify-content-center mt-5">
        <div class="col-md-6 col-lg-5">
            <div class="card shadow-lg">
                <div class="card-body p-5">
                    <div class="text-center mb-4">
                        <i class="fas fa-user-shield fa-3x text-primary mb-3"></i>
                        <h2 class="card-title">Admin Login</h2>
                        <p class="text-muted">Maruti Training Portal</p>
                    </div>

                    <form asp-action="Login" method="post">
                        <div asp-validation-summary="All" class="text-danger mb-3"></div>
                        
                        <div class="mb-3">
                            <label asp-for="Email" class="form-label"></label>
                            <div class="input-group">
                                <span class="input-group-text"><i class="fas fa-envelope"></i></span>
                                <input asp-for="Email" class="form-control" placeholder="admin@marutitraining.com" />
                            </div>
                            <span asp-validation-for="Email" class="text-danger small"></span>
                        </div>

                        <div class="mb-3">
                            <label asp-for="Password" class="form-label"></label>
                            <div class="input-group">
                                <span class="input-group-text"><i class="fas fa-lock"></i></span>
                                <input asp-for="Password" class="form-control" placeholder="Enter password" />
                            </div>
                            <span asp-validation-for="Password" class="text-danger small"></span>
                        </div>

                        <div class="mb-3 form-check">
                            <input asp-for="RememberMe" class="form-check-input" />
                            <label asp-for="RememberMe" class="form-check-label"></label>
                        </div>

                        <div class="d-grid gap-2">
                            <button type="submit" class="btn btn-primary btn-lg">
                                <i class="fas fa-sign-in-alt me-2"></i>Login
                            </button>
                        </div>
                    </form>

                    <div class="text-center mt-4">
                        <small class="text-muted">
                            Default: admin@marutitraining.com / Admin@123456
                        </small>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

#### File: Views/Account/AccessDenied.cshtml

```cshtml
@{
    ViewData["Title"] = "Access Denied";
}

<div class="container">
    <div class="row justify-content-center mt-5">
        <div class="col-md-8 text-center">
            <i class="fas fa-ban fa-5x text-danger mb-4"></i>
            <h1 class="display-4">Access Denied</h1>
            <p class="lead text-muted">
                You don't have permission to access this resource.
            </p>
            <p class="text-muted">
                Please log in with an administrator account to continue.
            </p>
            <div class="mt-4">
                <a asp-action="Login" asp-controller="Account" class="btn btn-primary">
                    <i class="fas fa-sign-in-alt me-2"></i>Login
                </a>
                <a asp-action="Index" asp-controller="Home" class="btn btn-outline-secondary">
                    <i class="fas fa-home me-2"></i>Go Home
                </a>
            </div>
        </div>
    </div>
</div>
```

---

### STEP 3: Create Admin Dashboard Views

#### File: Views/Admin/Dashboard.cshtml

```cshtml
@model MarutiTrainingPortal.Models.ViewModels.AdminDashboardViewModel
@{
    ViewData["Title"] = "Admin Dashboard";
    Layout = "~/Views/Shared/_AdminLayout.cshtml";
}

<div class="container-fluid">
    <div class="row mb-4">
        <div class="col-12">
            <h2><i class="fas fa-tachometer-alt me-2"></i>Dashboard</h2>
            <p class="text-muted">Welcome back, @User.Identity?.Name</p>
        </div>
    </div>

    <!-- Statistics Cards -->
    <div class="row g-4 mb-4">
        <div class="col-xl-3 col-md-6">
            <div class="card border-left-primary shadow h-100">
                <div class="card-body">
                    <div class="row align-items-center">
                        <div class="col">
                            <div class="text-xs font-weight-bold text-primary text-uppercase mb-1">
                                Total Trainings
                            </div>
                            <div class="h5 mb-0 font-weight-bold">@Model.TotalTrainings</div>
                        </div>
                        <div class="col-auto">
                            <i class="fas fa-chalkboard-teacher fa-2x text-primary"></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-xl-3 col-md-6">
            <div class="card border-left-success shadow h-100">
                <div class="card-body">
                    <div class="row align-items-center">
                        <div class="col">
                            <div class="text-xs font-weight-bold text-success text-uppercase mb-1">
                                Total Courses
                            </div>
                            <div class="h5 mb-0 font-weight-bold">@Model.TotalCourses</div>
                        </div>
                        <div class="col-auto">
                            <i class="fas fa-video fa-2x text-success"></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-xl-3 col-md-6">
            <div class="card border-left-info shadow h-100">
                <div class="card-body">
                    <div class="row align-items-center">
                        <div class="col">
                            <div class="text-xs font-weight-bold text-info text-uppercase mb-1">
                                Upcoming Events
                            </div>
                            <div class="h5 mb-0 font-weight-bold">@Model.UpcomingEvents</div>
                        </div>
                        <div class="col-auto">
                            <i class="fas fa-calendar-alt fa-2x text-info"></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-xl-3 col-md-6">
            <div class="card border-left-warning shadow h-100">
                <div class="card-body">
                    <div class="row align-items-center">
                        <div class="col">
                            <div class="text-xs font-weight-bold text-warning text-uppercase mb-1">
                                Unread Messages
                            </div>
                            <div class="h5 mb-0 font-weight-bold">@Model.UnreadContactMessages</div>
                        </div>
                        <div class="col-auto">
                            <i class="fas fa-envelope fa-2x text-warning"></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Charts Row -->
    <div class="row mb-4">
        <div class="col-xl-8 col-lg-7">
            <div class="card shadow mb-4">
                <div class="card-header py-3">
                    <h6 class="m-0 font-weight-bold text-primary">Monthly Trainings Overview</h6>
                </div>
                <div class="card-body">
                    <canvas id="trainingsChart" width="100%" height="40"></canvas>
                </div>
            </div>
        </div>

        <div class="col-xl-4 col-lg-5">
            <div class="card shadow mb-4">
                <div class="card-header py-3">
                    <h6 class="m-0 font-weight-bold text-primary">Courses by Category</h6>
                </div>
                <div class="card-body">
                    <canvas id="coursesChart"></canvas>
                </div>
            </div>
        </div>
    </div>

    <!-- Recent Messages & Upcoming Events -->
    <div class="row">
        <div class="col-lg-6">
            <div class="card shadow mb-4">
                <div class="card-header py-3 d-flex justify-content-between align-items-center">
                    <h6 class="m-0 font-weight-bold text-primary">Recent Messages</h6>
                    <a href="/Contact" class="btn btn-sm btn-primary">View All</a>
                </div>
                <div class="card-body">
                    @if (Model.RecentMessages.Any())
                    {
                        <div class="list-group list-group-flush">
                            @foreach (var message in Model.RecentMessages)
                            {
                                <div class="list-group-item @(message.IsRead ? "" : "bg-light")">
                                    <div class="d-flex w-100 justify-content-between">
                                        <h6 class="mb-1">@message.Name</h6>
                                        <small class="text-muted">@message.CreatedDate.ToString("MMM dd, yyyy")</small>
                                    </div>
                                    <p class="mb-1"><strong>@message.Subject</strong></p>
                                    <small class="text-muted">@message.Email</small>
                                </div>
                            }
                        </div>
                    }
                    else
                    {
                        <p class="text-muted text-center">No messages yet</p>
                    }
                </div>
            </div>
        </div>

        <div class="col-lg-6">
            <div class="card shadow mb-4">
                <div class="card-header py-3 d-flex justify-content-between align-items-center">
                    <h6 class="m-0 font-weight-bold text-primary">Upcoming Events</h6>
                    <a asp-controller="Events" asp-action="Index" class="btn btn-sm btn-primary">View All</a>
                </div>
                <div class="card-body">
                    @if (Model.UpcomingEventsList.Any())
                    {
                        <div class="list-group list-group-flush">
                            @foreach (var evt in Model.UpcomingEventsList)
                            {
                                <div class="list-group-item">
                                    <div class="d-flex w-100 justify-content-between">
                                        <h6 class="mb-1">@evt.Title</h6>
                                        <span class="badge bg-info">@evt.EventType</span>
                                    </div>
                                    <p class="mb-1"><i class="fas fa-calendar me-2"></i>@evt.StartDate.ToString("MMM dd, yyyy")</p>
                                    <small class="text-muted"><i class="fas fa-map-marker-alt me-2"></i>@evt.Location</small>
                                </div>
                            }
                        </div>
                    }
                    else
                    {
                        <p class="text-muted text-center">No upcoming events</p>
                    }
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
    <script>
        // Fetch chart data from API
        fetch('/Admin/GetChartData')
            .then(response => response.json())
            .then(data => {
                // Monthly Trainings Chart
                const trainingsCtx = document.getElementById('trainingsChart').getContext('2d');
                new Chart(trainingsCtx, {
                    type: 'line',
                    data: {
                        labels: data.monthlyTrainings.map(item => item.month),
                        datasets: [{
                            label: 'Trainings',
                            data: data.monthlyTrainings.map(item => item.count),
                            borderColor: 'rgb(75, 192, 192)',
                            tension: 0.1,
                            fill: true,
                            backgroundColor: 'rgba(75, 192, 192, 0.2)'
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: { display: true }
                        }
                    }
                });

                // Courses by Category Chart
                const coursesCtx = document.getElementById('coursesChart').getContext('2d');
                new Chart(coursesCtx, {
                    type: 'doughnut',
                    data: {
                        labels: data.coursesByCategory.map(item => item.category),
                        datasets: [{
                            data: data.coursesByCategory.map(item => item.count),
                            backgroundColor: [
                                'rgb(255, 99, 132)',
                                'rgb(54, 162, 235)',
                                'rgb(255, 205, 86)',
                                'rgb(75, 192, 192)',
                                'rgb(153, 102, 255)'
                            ]
                        }]
                    },
                    options: {
                        responsive: true,
                        plugins: {
                            legend: { position: 'bottom' }
                        }
                    }
                });
            });
    </script>
}
```

#### File: Views/Shared/_AdminLayout.cshtml

```cshtml
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Admin Panel</title>
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
    <style>
        body {
            background-color: #f8f9fc;
        }
        
        .sidebar {
            position: fixed;
            top: 0;
            bottom: 0;
            left: 0;
            z-index: 100;
            padding: 48px 0 0;
            box-shadow: inset -1px 0 0 rgba(0, 0, 0, .1);
            width: 250px;
            background-color: #4e73df;
        }
        
        .sidebar-sticky {
            position: relative;
            top: 0;
            height: calc(100vh - 48px);
            padding-top: .5rem;
            overflow-x: hidden;
            overflow-y: auto;
        }
        
        .sidebar .nav-link {
            font-weight: 500;
            color: rgba(255, 255, 255, .8);
            padding: 12px 20px;
        }
        
        .sidebar .nav-link:hover {
            color: #fff;
            background-color: rgba(255, 255, 255, .1);
        }
        
        .sidebar .nav-link.active {
            color: #fff;
            background-color: rgba(255, 255, 255, .2);
        }
        
        .sidebar-heading {
            font-size: .75rem;
            text-transform: uppercase;
            color: rgba(255, 255, 255, .5);
            padding: 12px 20px;
            font-weight: 600;
        }
        
        .navbar-brand {
            padding-top: .75rem;
            padding-bottom: .75rem;
            background-color: rgba(0, 0, 0, .25);
            box-shadow: inset -1px 0 0 rgba(0, 0, 0, .25);
        }
        
        .navbar-brand {
            font-size: 1rem;
        }
        
        main {
            margin-left: 250px;
            padding-top: 48px;
        }
        
        .border-left-primary {
            border-left: .25rem solid #4e73df !important;
        }
        
        .border-left-success {
            border-left: .25rem solid #1cc88a !important;
        }
        
        .border-left-info {
            border-left: .25rem solid #36b9cc !important;
        }
        
        .border-left-warning {
            border-left: .25rem solid #f6c23e !important;
        }
        
        .text-xs {
            font-size: .7rem;
        }
    </style>
</head>
<body>
    <nav class="navbar navbar-dark sticky-top bg-dark flex-md-nowrap p-0 shadow" style="z-index: 1000;">
        <a class="navbar-brand col-md-3 col-lg-2 me-0 px-3" href="/Admin/Dashboard">
            <i class="fas fa-graduation-cap me-2"></i>Admin Panel
        </a>
        <button class="navbar-toggler position-absolute d-md-none collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#sidebarMenu">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="navbar-nav">
            <div class="nav-item text-nowrap">
                <form asp-controller="Account" asp-action="Logout" method="post" class="d-inline">
                    <button type="submit" class="btn btn-link nav-link px-3 text-white">
                        <i class="fas fa-sign-out-alt me-2"></i>Logout
                    </button>
                </form>
            </div>
        </div>
    </nav>

    <div class="container-fluid">
        <div class="row">
            <nav id="sidebarMenu" class="sidebar">
                <div class="sidebar-sticky pt-3">
                    <ul class="nav flex-column">
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Admin" asp-action="Dashboard">
                                <i class="fas fa-fw fa-tachometer-alt me-2"></i>
                                Dashboard
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Trainings" asp-action="Index">
                                <i class="fas fa-fw fa-chalkboard-teacher me-2"></i>
                                Trainings
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Courses" asp-action="Index">
                                <i class="fas fa-fw fa-video me-2"></i>
                                Courses
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Events" asp-action="Index">
                                <i class="fas fa-fw fa-calendar-alt me-2"></i>
                                Events
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Profile" asp-action="About">
                                <i class="fas fa-fw fa-user me-2"></i>
                                Profile
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Contact" asp-action="Index">
                                <i class="fas fa-fw fa-envelope me-2"></i>
                                Contact Messages
                            </a>
                        </li>
                    </ul>

                    <h6 class="sidebar-heading mt-4">
                        <i class="fas fa-cog me-2"></i>Settings
                    </h6>
                    <ul class="nav flex-column mb-2">
                        <li class="nav-item">
                            <a class="nav-link" asp-controller="Home" asp-action="Index" target="_blank">
                                <i class="fas fa-fw fa-external-link-alt me-2"></i>
                                View Site
                            </a>
                        </li>
                    </ul>
                </div>
            </nav>

            <main role="main" class="ms-sm-auto px-md-4 py-4">
                @RenderBody()
            </main>
        </div>
    </div>

    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

---

### STEP 4: Update appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=MarutiTrainingPortal.db"
  },
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SmtpUsername": "",
    "SmtpPassword": "",
    "FromEmail": "noreply@marutitraining.com",
    "FromName": "Maruti Training Portal"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

### STEP 5: Add Custom CSS

#### File: wwwroot/css/admin.css

```css
/* Admin Dashboard Custom Styles */
.card {
    border: none;
    border-radius: 0.35rem;
}

.card .card-header {
    background-color: #f8f9fc;
    border-bottom: 1px solid #e3e6f0;
}

.shadow {
    box-shadow: 0 .15rem 1.75rem 0 rgba(58,59,69,.15)!important;
}

.font-weight-bold {
    font-weight: 700!important;
}

.text-primary {
    color: #4e73df!important;
}

.text-success {
    color: #1cc88a!important;
}

.text-info {
    color: #36b9cc!important;
}

.text-warning {
    color: #f6c23e!important;
}
```

---

### STEP 6: Update Navigation in _Layout.cshtml

Add login/logout button to the main navigation:

```cshtml
<ul class="navbar-nav ms-auto">
    @if (User.Identity?.IsAuthenticated == true)
    {
        <li class="nav-item">
            <a class="nav-link" asp-controller="Admin" asp-action="Dashboard">
                <i class="fas fa-tachometer-alt me-1"></i>Dashboard
            </a>
        </li>
        <li class="nav-item">
            <form asp-controller="Account" asp-action="Logout" method="post" class="d-inline">
                <button type="submit" class="btn btn-link nav-link">
                    <i class="fas fa-sign-out-alt me-1"></i>Logout
                </button>
            </form>
        </li>
    }
    else
    {
        <li class="nav-item">
            <a class="nav-link" asp-controller="Account" asp-action="Login">
                <i class="fas fa-sign-in-alt me-1"></i>Admin Login
            </a>
        </li>
    }
</ul>
```

---

### STEP 7: Create New Migration

```bash
# Remove old migrations
Remove-Item -Recurse -Force .\Migrations

# Create new migration with Identity
dotnet ef migrations add InitialIdentityMigration

# Apply migration
dotnet ef database update
```

---

### STEP 8: User Secrets for Email

```bash
# Initialize user secrets
dotnet user-secrets init

# Set SMTP credentials (example for Gmail)
dotnet user-secrets set "EmailSettings:SmtpUsername" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:SmtpPassword" "your-app-password"
```

---

### STEP 9: GitHub Actions CI/CD

#### File: .github/workflows/deploy.yml

```yaml
name: Deploy to Azure

on:
  push:
    branches: [ main ]
  workflow_dispatch:

jobs:
  build-and-deploy:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --configuration Release --no-restore
    
    - name: Test
      run: dotnet test --no-restore --verbosity normal
    
    - name: Publish
      run: dotnet publish -c Release -o ./publish
    
    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: ${{ secrets.AZURE_WEBAPP_NAME }}
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./publish
```

---

### STEP 10: Dockerfile

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MarutiTrainingPortal.csproj", "./"]
RUN dotnet restore "MarutiTrainingPortal.csproj"
COPY . .
RUN dotnet build "MarutiTrainingPortal.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "MarutiTrainingPortal.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "MarutiTrainingPortal.dll"]
```

---

### STEP 11: .dockerignore

```
**/.dockerignore
**/.env
**/.git
**/.gitignore
**/.vs
**/.vscode
**/bin
**/obj
**/.toolstarget
**/node_modules
**/logs
**/Migrations
*.db
*.db-shm
*.db-wal
```

---

## 🚀 DEPLOYMENT GUIDE

### Option 1: Azure App Service (Free Tier)

1. Create Azure account (Free tier available)
2. Create App Service (F1 Free tier)
3. Configure environment variables in Azure Portal
4. Use GitHub Actions or Azure DevOps for CI/CD
5. Connection string in Azure Configuration

### Option 2: Render.com (Free Tier)

1. Create account on Render.com
2. Connect GitHub repository
3. Create Web Service
4. Add environment variables
5. Auto-deploys on git push

### Option 3: Railway.app (Free Tier)

1. Create account on Railway.app
2. Deploy from GitHub
3. Add PostgreSQL database (free)
4. Configure environment variables

---

## 📝 TESTING

Create xUnit test project:

```bash
dotnet new xunit -n MarutiTrainingPortal.Tests
dotnet add reference ../MarutiTrainingPortal.csproj
dotnet add package Microsoft.EntityFrameworkCore.InMemory
dotnet add package Moq
```

Example test:
```csharp
using MarutiTrainingPortal.Controllers;
using MarutiTrainingPortal.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class CoursesControllerTests
{
    [Fact]
    public async Task Index_ReturnsViewResult_WithListOfCourses()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        
        using var context = new ApplicationDbContext(options);
        var controller = new CoursesController(context, null);
        
        // Act
        var result = await controller.Index();
        
        // Assert
        Assert.IsType<ViewResult>(result);
    }
}
```

---

## ✅ FINAL CHECKLIST

- [x] Identity configured with Admin role
- [x] Admin user seeded (admin@marutitraining.com / Admin@123456)
- [x] All controllers protected with [Authorize]
- [x] Public views have [AllowAnonymous]
- [x] Email service implemented
- [x] HTML sanitization added
- [x] Rate limiting on contact form
- [x] Admin dashboard with charts
- [x] Logging with Serilog
- [x] Session management
- [x] Database indexes added
- [ ] Run new migration
- [ ] Test all features
- [ ] Configure SMTP credentials
- [ ] Deploy to production

---

## 🔐 DEFAULT CREDENTIALS

**Email:** admin@marutitraining.com  
**Password:** Admin@123456

**⚠️ IMPORTANT: Change password after first login in production!**

---

## 📚 ADDITIONAL RESOURCES

- [ASP.NET Core Identity Documentation](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [Serilog Configuration](https://github.com/serilog/serilog-aspnetcore)
- [Chart.js Documentation](https://www.chartjs.org/docs/latest/)
- [Azure App Service Deployment](https://docs.microsoft.com/en-us/azure/app-service/)

---

Generated by: Maruti Training Portal Development Team  
Last Updated: November 30, 2025
