# Production JSON Fix Validation Script
# Run this in PowerShell after deployment to verify everything works

param(
    [Parameter(Mandatory=$true)]
    [string]$SiteUrl
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "JSON DATA PRODUCTION VALIDATION" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = $SiteUrl.TrimEnd('/')
$allTestsPassed = $true

# Test 1: JSON Diagnostics Endpoint
Write-Host "Test 1: JSON Diagnostics Endpoint" -ForegroundColor Yellow
Write-Host "Checking: $baseUrl/json-diagnostics" -ForegroundColor Gray
try {
    $diagnostics = Invoke-RestMethod -Uri "$baseUrl/json-diagnostics" -Method Get -ErrorAction Stop
    
    if ($diagnostics.JsonDataExists -eq $true) {
        Write-Host "  ✅ JsonData folder exists" -ForegroundColor Green
    } else {
        Write-Host "  ❌ JsonData folder NOT FOUND" -ForegroundColor Red
        $allTestsPassed = $false
    }
    
    if ($diagnostics.JsonFileCount -gt 0) {
        Write-Host "  ✅ Found $($diagnostics.JsonFileCount) JSON files" -ForegroundColor Green
        
        # List all files
        Write-Host "  📁 JSON Files:" -ForegroundColor Cyan
        foreach ($file in $diagnostics.JsonFiles) {
            $sizeKB = [math]::Round($file.Size / 1024, 2)
            $readable = if ($file.Readable) { "✓" } else { "✗" }
            Write-Host "     $readable $($file.Name) - $sizeKB KB" -ForegroundColor Gray
        }
    } else {
        Write-Host "  ❌ No JSON files found" -ForegroundColor Red
        $allTestsPassed = $false
    }
    
    Write-Host "  📍 Environment: $($diagnostics.Environment)" -ForegroundColor Gray
    Write-Host "  📍 ContentRootPath: $($diagnostics.ContentRootPath)" -ForegroundColor Gray
    
} catch {
    Write-Host "  ❌ Failed to access diagnostics endpoint" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
    $allTestsPassed = $false
}
Write-Host ""

# Test 2: Health Endpoint
Write-Host "Test 2: Health Endpoint" -ForegroundColor Yellow
Write-Host "Checking: $baseUrl/health" -ForegroundColor Gray
try {
    $health = Invoke-RestMethod -Uri "$baseUrl/health" -Method Get -ErrorAction Stop
    
    if ($health.status -eq "healthy") {
        Write-Host "  ✅ Application is healthy" -ForegroundColor Green
    } else {
        Write-Host "  ⚠️  Application status: $($health.status)" -ForegroundColor Yellow
    }
    
    if ($health.dataLoaded -eq $true) {
        Write-Host "  ✅ Data successfully loaded" -ForegroundColor Green
    } else {
        Write-Host "  ❌ Data NOT loaded" -ForegroundColor Red
        $allTestsPassed = $false
    }
    
    Write-Host "  📊 Data Counts:" -ForegroundColor Cyan
    Write-Host "     Courses: $($health.counts.courses)" -ForegroundColor Gray
    Write-Host "     Events: $($health.counts.events)" -ForegroundColor Gray
    Write-Host "     Profiles: $($health.counts.profiles)" -ForegroundColor Gray
    Write-Host "     Images: $($health.counts.images)" -ForegroundColor Gray
    
    if ($health.counts.courses -eq 0 -and $health.counts.events -eq 0) {
        Write-Host "  ⚠️  WARNING: All counts are zero - data may not be loading" -ForegroundColor Yellow
        $allTestsPassed = $false
    }
    
} catch {
    Write-Host "  ❌ Failed to access health endpoint" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
    $allTestsPassed = $false
}
Write-Host ""

# Test 3: Events Page
Write-Host "Test 3: Events Page" -ForegroundColor Yellow
Write-Host "Checking: $baseUrl/events" -ForegroundColor Gray
try {
    $eventsPage = Invoke-WebRequest -Uri "$baseUrl/events" -Method Get -ErrorAction Stop
    
    if ($eventsPage.StatusCode -eq 200) {
        Write-Host "  ✅ Events page loads successfully" -ForegroundColor Green
        
        # Check for common error indicators
        if ($eventsPage.Content -match "No Events Found|Unable to load") {
            Write-Host "  ⚠️  WARNING: Page shows empty state message" -ForegroundColor Yellow
        } else {
            Write-Host "  ✅ Page appears to have content" -ForegroundColor Green
        }
    } else {
        Write-Host "  ❌ Events page returned status: $($eventsPage.StatusCode)" -ForegroundColor Red
        $allTestsPassed = $false
    }
} catch {
    Write-Host "  ❌ Failed to access events page" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
    $allTestsPassed = $false
}
Write-Host ""

# Test 4: Courses Page
Write-Host "Test 4: Courses Page" -ForegroundColor Yellow
Write-Host "Checking: $baseUrl/courses" -ForegroundColor Gray
try {
    $coursesPage = Invoke-WebRequest -Uri "$baseUrl/courses" -Method Get -ErrorAction Stop
    
    if ($coursesPage.StatusCode -eq 200) {
        Write-Host "  ✅ Courses page loads successfully" -ForegroundColor Green
        
        if ($coursesPage.Content -match "No Courses Found|Unable to load") {
            Write-Host "  ⚠️  WARNING: Page shows empty state message" -ForegroundColor Yellow
        } else {
            Write-Host "  ✅ Page appears to have content" -ForegroundColor Green
        }
    } else {
        Write-Host "  ❌ Courses page returned status: $($coursesPage.StatusCode)" -ForegroundColor Red
        $allTestsPassed = $false
    }
} catch {
    Write-Host "  ❌ Failed to access courses page" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
    $allTestsPassed = $false
}
Write-Host ""

# Test 5: Home Page
Write-Host "Test 5: Home Page" -ForegroundColor Yellow
Write-Host "Checking: $baseUrl/" -ForegroundColor Gray
try {
    $homePage = Invoke-WebRequest -Uri "$baseUrl/" -Method Get -ErrorAction Stop
    
    if ($homePage.StatusCode -eq 200) {
        Write-Host "  ✅ Home page loads successfully" -ForegroundColor Green
    } else {
        Write-Host "  ❌ Home page returned status: $($homePage.StatusCode)" -ForegroundColor Red
        $allTestsPassed = $false
    }
} catch {
    Write-Host "  ❌ Failed to access home page" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
    $allTestsPassed = $false
}
Write-Host ""

# Summary
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "VALIDATION SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if ($allTestsPassed) {
    Write-Host "✅ ALL TESTS PASSED" -ForegroundColor Green
    Write-Host ""
    Write-Host "Your production deployment is working correctly!" -ForegroundColor Green
    Write-Host "JSON data is loading and all endpoints are accessible." -ForegroundColor Green
    exit 0
} else {
    Write-Host "❌ SOME TESTS FAILED" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting steps:" -ForegroundColor Yellow
    Write-Host "1. Visit $baseUrl/json-diagnostics for detailed file information" -ForegroundColor White
    Write-Host "2. Check application logs for error messages" -ForegroundColor White
    Write-Host "3. Verify .csproj has CopyToOutputDirectory=Always" -ForegroundColor White
    Write-Host "4. Ensure JsonData folder exists in deployment" -ForegroundColor White
    Write-Host "5. Check file permissions on hosting server" -ForegroundColor White
    Write-Host ""
    Write-Host "See DEPLOYMENT_QUICK_START.md for detailed troubleshooting guide" -ForegroundColor Cyan
    exit 1
}

<#
.SYNOPSIS
    Validates production deployment of JSON data fix

.DESCRIPTION
    This script runs comprehensive tests against a deployed website to verify
    that JSON data is loading correctly. It checks diagnostic endpoints,
    health status, and actual page functionality.

.PARAMETER SiteUrl
    The base URL of the deployed website (e.g., https://yoursite.com)

.EXAMPLE
    .\Validate-Production.ps1 -SiteUrl "https://yoursite.com"

.EXAMPLE
    .\Validate-Production.ps1 -SiteUrl "https://yoursite.azurewebsites.net"

.NOTES
    - Requires PowerShell 5.1 or later
    - Requires internet connectivity to target site
    - Run after every deployment to ensure data loading works
#>
