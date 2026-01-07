# Azure Deployment Guide - SkillTech Navigator

## 🚀 Deploy to Azure App Service

This guide will walk you through deploying your SkillTech Navigator to Microsoft Azure.

---

## Prerequisites

- [ ] Azure Account ([Create free account](https://azure.microsoft.com/free/))
- [ ] Azure CLI installed ([Download](https://docs.microsoft.com/cli/azure/install-azure-cli))
- [ ] Google Gemini API Key
- [ ] Application builds successfully locally

---

## Option 1: Deploy via Azure CLI (Recommended)

### Step 1: Login to Azure

```powershell
az login
```

This will open a browser window for authentication.

### Step 2: Create Resource Group

```powershell
az group create `
  --name SkillTechNavigatorRG `
  --location eastus
```

### Step 3: Create App Service Plan

```powershell
az appservice plan create `
  --name SkillTechNavigatorPlan `
  --resource-group SkillTechNavigatorRG `
  --sku B1 `
  --is-linux
```

**Pricing Tiers:**
- **B1** (Basic): ~$13/month - Good for development/testing
- **S1** (Standard): ~$70/month - Recommended for production
- **F1** (Free): Available but very limited (60 minutes/day)

### Step 4: Create Web App

```powershell
az webapp create `
  --name skilltech-navigator-[YOUR-UNIQUE-NAME] `
  --resource-group SkillTechNavigatorRG `
  --plan SkillTechNavigatorPlan `
  --runtime "DOTNET|8.0"
```

**Note**: Replace `[YOUR-UNIQUE-NAME]` with something unique (e.g., your initials + numbers)

### Step 5: Configure Application Settings

```powershell
az webapp config appsettings set `
  --name skilltech-navigator-[YOUR-UNIQUE-NAME] `
  --resource-group SkillTechNavigatorRG `
  --settings Gemini__ApiKey="YOUR_GEMINI_API_KEY_HERE"
```

### Step 6: Publish Application

```powershell
# Navigate to project directory
cd c:\Users\Skill\Desktop\skilltechBot

# Publish to folder
dotnet publish -c Release -o ./publish

# Create deployment zip
Compress-Archive -Path ./publish/* -DestinationPath ./deploy.zip -Force

# Deploy to Azure
az webapp deploy `
  --name skilltech-navigator-[YOUR-UNIQUE-NAME] `
  --resource-group SkillTechNavigatorRG `
  --src-path ./deploy.zip `
  --type zip
```

### Step 7: Browse Your Application

```powershell
az webapp browse `
  --name skilltech-navigator-[YOUR-UNIQUE-NAME] `
  --resource-group SkillTechNavigatorRG
```

Your app will be available at:
`https://skilltech-navigator-[YOUR-UNIQUE-NAME].azurewebsites.net`

---

## Option 2: Deploy via Azure Portal

### Step 1: Create Web App

1. Go to [Azure Portal](https://portal.azure.com)
2. Click "Create a resource"
3. Search for "Web App"
4. Click "Create"

**Configuration:**
- **Subscription**: Your subscription
- **Resource Group**: Create new "SkillTechNavigatorRG"
- **Name**: skilltech-navigator-[unique-name]
- **Publish**: Code
- **Runtime stack**: .NET 8 (LTS)
- **Operating System**: Linux
- **Region**: East US (or closest to you)
- **App Service Plan**: Create new (Basic B1)

5. Click "Review + Create"
6. Click "Create"

### Step 2: Configure Application Settings

1. Navigate to your Web App in Azure Portal
2. Click "Configuration" in the left menu
3. Under "Application settings", click "+ New application setting"
4. Add:
   - **Name**: `Gemini:ApiKey`
   - **Value**: Your Gemini API key
5. Click "OK", then "Save"

### Step 3: Deploy Code

**Method A: Visual Studio**
1. Right-click project in Solution Explorer
2. Click "Publish"
3. Choose "Azure"
4. Choose "Azure App Service (Linux)"
5. Select your app
6. Click "Publish"

**Method B: VS Code**
1. Install "Azure App Service" extension
2. Click Azure icon in sidebar
3. Right-click your app
4. Select "Deploy to Web App"
5. Select your publish folder

**Method C: GitHub Actions** (see below)

---

## Option 3: Deploy via GitHub Actions (CI/CD)

### Step 1: Push Code to GitHub

```bash
cd c:\Users\Skill\Desktop\skilltechBot
git init
git add .
git commit -m "Initial commit"
git branch -M main
git remote add origin https://github.com/YOUR-USERNAME/skilltech-navigator.git
git push -u origin main
```

### Step 2: Get Publish Profile

```powershell
az webapp deployment list-publishing-profiles `
  --name skilltech-navigator-[YOUR-UNIQUE-NAME] `
  --resource-group SkillTechNavigatorRG `
  --xml > publishprofile.xml
```

### Step 3: Add GitHub Secret

1. Go to your GitHub repository
2. Click "Settings" → "Secrets and variables" → "Actions"
3. Click "New repository secret"
4. Name: `AZURE_WEBAPP_PUBLISH_PROFILE`
5. Value: Contents of `publishprofile.xml`
6. Click "Add secret"

### Step 4: Create GitHub Actions Workflow

Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy to Azure

on:
  push:
    branches: [ main ]
  workflow_dispatch:

env:
  AZURE_WEBAPP_NAME: skilltech-navigator-[YOUR-UNIQUE-NAME]
  DOTNET_VERSION: '8.0.x'

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --configuration Release --no-restore
    
    - name: Publish
      run: dotnet publish -c Release -o ${{env.DOTNET_ROOT}}/myapp
    
    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: ${{ env.AZURE_WEBAPP_NAME }}
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ${{env.DOTNET_ROOT}}/myapp
```

### Step 5: Commit and Push

```bash
git add .github/workflows/deploy.yml
git commit -m "Add GitHub Actions deployment"
git push
```

Now every push to `main` will automatically deploy to Azure!

---

## Post-Deployment Configuration

### Enable HTTPS Only

```powershell
az webapp update `
  --name skilltech-navigator-[YOUR-UNIQUE-NAME] `
  --resource-group SkillTechNavigatorRG `
  --set httpsOnly=true
```

### Enable Application Insights (Monitoring)

```powershell
az monitor app-insights component create `
  --app skilltech-navigator-insights `
  --location eastus `
  --resource-group SkillTechNavigatorRG

az webapp config appsettings set `
  --name skilltech-navigator-[YOUR-UNIQUE-NAME] `
  --resource-group SkillTechNavigatorRG `
  --settings APPLICATIONINSIGHTS_CONNECTION_STRING="<connection-string>"
```

### Configure Custom Domain (Optional)

1. Go to Azure Portal → Your Web App
2. Click "Custom domains"
3. Click "Add custom domain"
4. Follow the instructions to verify domain ownership
5. Add DNS records as instructed

### Enable Automatic Scaling (Optional)

```powershell
az monitor autoscale create `
  --resource-group SkillTechNavigatorRG `
  --resource skilltech-navigator-[YOUR-UNIQUE-NAME] `
  --resource-type Microsoft.Web/sites `
  --name autoscale-skilltech `
  --min-count 1 `
  --max-count 3 `
  --count 1
```

---

## Environment Variables Reference

Set these in Azure App Service Configuration:

| Setting Name | Description | Example |
|-------------|-------------|---------|
| `Gemini:ApiKey` | Your Google Gemini API key | `AIzaSy...` |
| `ASPNETCORE_ENVIRONMENT` | Environment name | `Production` |
| `APPLICATIONINSIGHTS_CONNECTION_STRING` | Monitoring connection | `InstrumentationKey=...` |

---

## Troubleshooting

### Issue: "Application Error" after deployment

**Solution:**
1. Check logs:
   ```powershell
   az webapp log tail `
     --name skilltech-navigator-[YOUR-UNIQUE-NAME] `
     --resource-group SkillTechNavigatorRG
   ```
2. Verify API key is set correctly
3. Check Application Insights for errors

### Issue: API calls failing

**Solution:**
1. Verify `Gemini:ApiKey` is set in Configuration
2. Check that outbound connections are allowed
3. Verify API key is valid in Google AI Studio

### Issue: Slow performance

**Solution:**
1. Upgrade to S1 or higher App Service Plan
2. Enable Application Insights to identify bottlenecks
3. Consider adding Redis cache for session state

### Issue: Application won't start

**Solution:**
1. Check runtime version matches (.NET 8.0)
2. Verify all dependencies are included in publish
3. Check startup logs in App Service logs

---

## Monitoring & Maintenance

### View Logs

```powershell
# Real-time logs
az webapp log tail `
  --name skilltech-navigator-[YOUR-UNIQUE-NAME] `
  --resource-group SkillTechNavigatorRG

# Download logs
az webapp log download `
  --name skilltech-navigator-[YOUR-UNIQUE-NAME] `
  --resource-group SkillTechNavigatorRG `
  --log-file logs.zip
```

### Check Health

Visit: `https://skilltech-navigator-[YOUR-UNIQUE-NAME].azurewebsites.net/api/chat/health`

Should return:
```json
{
  "status": "healthy",
  "timestamp": "2026-01-03T10:00:00Z"
}
```

### Monitor Costs

```powershell
az consumption usage list `
  --start-date 2026-01-01 `
  --end-date 2026-01-31
```

Or check in Azure Portal → Cost Management

---

## Security Best Practices

### 1. Use Azure Key Vault for Secrets

```powershell
# Create Key Vault
az keyvault create `
  --name skilltech-keyvault `
  --resource-group SkillTechNavigatorRG `
  --location eastus

# Add secret
az keyvault secret set `
  --vault-name skilltech-keyvault `
  --name GeminiApiKey `
  --value "YOUR_API_KEY"

# Enable managed identity for app
az webapp identity assign `
  --name skilltech-navigator-[YOUR-UNIQUE-NAME] `
  --resource-group SkillTechNavigatorRG

# Grant access to Key Vault
az keyvault set-policy `
  --name skilltech-keyvault `
  --object-id <managed-identity-principal-id> `
  --secret-permissions get
```

### 2. Enable Authentication (Optional)

To require login:

```powershell
az webapp auth update `
  --name skilltech-navigator-[YOUR-UNIQUE-NAME] `
  --resource-group SkillTechNavigatorRG `
  --enabled true `
  --action LoginWithAzureActiveDirectory
```

### 3. Configure Firewall Rules

```powershell
az webapp config access-restriction add `
  --name skilltech-navigator-[YOUR-UNIQUE-NAME] `
  --resource-group SkillTechNavigatorRG `
  --rule-name "Allow-My-IP" `
  --action Allow `
  --ip-address YOUR.IP.ADDRESS.HERE/32 `
  --priority 100
```

---

## Cost Optimization

### Estimated Monthly Costs

| Tier | Price | Best For |
|------|-------|----------|
| **Free (F1)** | $0 | Testing only (very limited) |
| **Basic (B1)** | ~$13 | Development/staging |
| **Standard (S1)** | ~$70 | Production (small) |
| **Premium (P1v2)** | ~$150 | Production (enterprise) |

**Additional Costs:**
- Bandwidth: ~$0.12/GB (first 5 GB free)
- Application Insights: First 5 GB/month free
- Gemini API: 1M tokens/month free, then ~$0.01/1K tokens

### Tips to Reduce Costs

1. **Use Free Tier for Development**: Switch to Free (F1) plan for development
2. **Auto-shutdown**: Schedule app to stop during non-business hours
3. **Right-size**: Monitor usage and downgrade if possible
4. **Budget Alerts**: Set up cost alerts in Azure

```powershell
# Change to Free tier (development only!)
az appservice plan update `
  --name SkillTechNavigatorPlan `
  --resource-group SkillTechNavigatorRG `
  --sku FREE
```

---

## Backup & Disaster Recovery

### Enable Backup

```powershell
# Create storage account
az storage account create `
  --name skilltechbackup `
  --resource-group SkillTechNavigatorRG `
  --location eastus `
  --sku Standard_LRS

# Configure backup (requires Standard tier or higher)
az webapp config backup create `
  --resource-group SkillTechNavigatorRG `
  --webapp-name skilltech-navigator-[YOUR-UNIQUE-NAME] `
  --backup-name InitialBackup `
  --container-url <storage-container-url-with-sas>
```

---

## Cleanup (Delete Everything)

To remove all resources and stop charges:

```powershell
az group delete `
  --name SkillTechNavigatorRG `
  --yes `
  --no-wait
```

⚠️ **Warning**: This deletes everything permanently!

---

## Next Steps

After successful deployment:

- [ ] Test application at production URL
- [ ] Run through all test cases in TESTING.md
- [ ] Set up monitoring and alerts
- [ ] Configure backup strategy
- [ ] Share with users and gather feedback
- [ ] Monitor usage and costs

---

## Support Resources

- [Azure App Service Documentation](https://docs.microsoft.com/azure/app-service/)
- [Azure CLI Reference](https://docs.microsoft.com/cli/azure/)
- [Azure Pricing Calculator](https://azure.microsoft.com/pricing/calculator/)
- [Azure Support](https://azure.microsoft.com/support/)

---

**Congratulations on deploying to Azure! 🎉**
