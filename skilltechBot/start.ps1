# SkillTech Copilot - Quick Start Script
# This script helps you get started quickly

Write-Host "SkillTech Copilot - Quick Start" -ForegroundColor Cyan
Write-Host "====================================`n" -ForegroundColor Cyan

# Check if .NET is installed
Write-Host "Checking prerequisites..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET SDK $dotnetVersion installed" -ForegroundColor Green
} catch {
    Write-Host "❌ .NET SDK not found! Please install .NET 8.0 SDK" -ForegroundColor Red
    Write-Host "Download from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    exit 1
}

# Check if API key is configured
Write-Host "`nChecking configuration..." -ForegroundColor Yellow
$configPath = "appsettings.Development.json"
if (Test-Path $configPath) {
    $config = Get-Content $configPath | ConvertFrom-Json
    $apiKey = $config.Gemini.ApiKey
    
    if ($apiKey -eq "YOUR_GEMINI_API_KEY_HERE" -or $apiKey -eq "") {
        Write-Host "⚠️  Gemini API key not configured!" -ForegroundColor Red
        Write-Host ""
        Write-Host "To configure your API key:" -ForegroundColor Yellow
        Write-Host "1. Get your key from: https://makersuite.google.com/app/apikey" -ForegroundColor White
        Write-Host "2. Edit 'appsettings.Development.json'" -ForegroundColor White
        Write-Host "3. Replace 'YOUR_GEMINI_API_KEY_HERE' with your actual key" -ForegroundColor White
        Write-Host ""
        
        $response = Read-Host "Do you want to continue anyway? (y/n)"
        if ($response -ne "y") {
            Write-Host "Setup cancelled. Please configure your API key first." -ForegroundColor Yellow
            exit 0
        }
    } else {
        Write-Host "✅ API key configured" -ForegroundColor Green
    }
} else {
    Write-Host "❌ Configuration file not found!" -ForegroundColor Red
    exit 1
}

# Build the project
Write-Host "`nBuilding project..." -ForegroundColor Yellow
dotnet build --configuration Release --nologo --verbosity quiet

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed! Please check for errors above." -ForegroundColor Red
    exit 1
}

Write-Host "✅ Build successful" -ForegroundColor Green

# Display startup information
Write-Host "`n====================================`n" -ForegroundColor Cyan
Write-Host "Starting SkillTech Copilot..." -ForegroundColor Green
Write-Host ""
Write-Host "Access the application at:" -ForegroundColor Yellow
Write-Host "   HTTPS: https://localhost:5001" -ForegroundColor White
Write-Host "   HTTP:  http://localhost:5000" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop the application" -ForegroundColor Gray
Write-Host "`n====================================`n" -ForegroundColor Cyan

# Start the application
dotnet run --no-build --configuration Release
