# SkillTech Navigator - Setup Guide

## 🎯 Quick Setup (5 Minutes)

Follow these steps to get your SkillTech Navigator up and running!

---

## Step 1: Get Your Google Gemini API Key

1. **Visit Google AI Studio**
   - Go to: https://makersuite.google.com/app/apikey
   - Sign in with your Google account

2. **Create API Key**
   - Click the "Create API Key" button
   - Select "Create API key in new project" (or use an existing project)
   - Copy the generated API key

3. **Save Your Key**
   - Keep this key secure - you'll need it in the next step!

---

## Step 2: Configure the Application

1. **Open the project folder**
   ```
   c:\Users\Skill\Desktop\skilltechBot
   ```

2. **Edit appsettings.Development.json**
   - Open the file in any text editor
   - Find the line: `"ApiKey": "YOUR_GEMINI_API_KEY_HERE"`
   - Replace `YOUR_GEMINI_API_KEY_HERE` with your actual API key
   - Save the file

   Example:
   ```json
   {
     "Gemini": {
       "ApiKey": "AIzaSyAbCdEfGhIjKlMnOpQrStUvWxYz1234567"
     }
   }
   ```

---

## Step 3: Run the Application

### Option A: Using Visual Studio 2022
1. Open `SkillTechNavigator.sln` in Visual Studio
2. Press `F5` or click the green "Play" button
3. Your browser will automatically open to the application

### Option B: Using Command Line
1. Open PowerShell or Command Prompt
2. Navigate to the project folder:
   ```powershell
   cd c:\Users\Skill\Desktop\skilltechBot
   ```
3. Run the application:
   ```powershell
   dotnet run
   ```
4. Open your browser and go to: `https://localhost:5001`

---

## Step 4: Test the Application

### Basic Tests

1. **Open the application** in your browser
2. **Send a test message**: Click "Learn Azure" or type "Hello"
3. **Verify response**: You should receive a response from Maruti Makwana

### Advanced Tests

Test different conversation goals:

| What to Type | Expected Goal Badge |
|-------------|-------------------|
| "I want to learn Azure" | Course Recommendations |
| "How much does it cost?" | Sales |
| "I need help" | Support |
| "Tell me about yourself" | General |

---

## 🎨 Customization Guide

### Change Agent Name/Persona

Edit `Services/GeminiService.cs` and modify the `GetSystemPrompt()` method:

```csharp
private string GetSystemPrompt()
{
    return @"You are [YOUR NAME], the SkillTech Navigator...";
}
```

### Change UI Colors

Edit `Views/Home/Index.cshtml` and modify the CSS variables:

```css
:root {
    --azure-blue: #0078D4;    /* Change primary color */
    --azure-light: #50E6FF;   /* Change accent color */
    --azure-purple: #8661C5;  /* Change secondary color */
}
```

### Change Welcome Message

Edit `Views/Home/Index.cshtml` and find the welcome screen section:

```html
<h2>Welcome to SkillTech Navigator!</h2>
<p>Your custom welcome message here!</p>
```

---

## 🚀 Deployment to Azure

### Prerequisites
- Azure account (free tier works!)
- Azure CLI installed

### Quick Deploy

1. **Create App Service**
   ```bash
   az webapp create --name your-app-name --resource-group your-resource-group --plan your-plan --runtime "DOTNET|8.0"
   ```

2. **Set API Key**
   ```bash
   az webapp config appsettings set --name your-app-name --resource-group your-resource-group --settings Gemini__ApiKey="your-api-key"
   ```

3. **Publish**
   ```bash
   dotnet publish -c Release
   cd bin/Release/net8.0/publish
   zip -r publish.zip .
   az webapp deploy --name your-app-name --resource-group your-resource-group --src-path publish.zip
   ```

---

## 🔧 Troubleshooting

### Problem: "API Key not configured" error

**Solution:**
1. Check `appsettings.Development.json` has your API key
2. Restart the application
3. Verify no extra spaces in the API key

### Problem: Application won't start

**Solution:**
1. Verify .NET 8.0 SDK is installed: `dotnet --version`
2. Check for port conflicts (port 5001 must be free)
3. Try running: `dotnet clean` then `dotnet build`

### Problem: UI looks broken

**Solution:**
1. Clear browser cache (Ctrl+Shift+Delete)
2. Try a different browser
3. Check browser console for errors (F12)

### Problem: No response from AI

**Solution:**
1. Verify API key is correct
2. Check internet connection
3. Verify Gemini API quota hasn't been exceeded
4. Check browser console and application logs

---

## 📊 System Requirements

### Development
- **OS**: Windows 10/11, macOS, or Linux
- **RAM**: 4GB minimum, 8GB recommended
- **Storage**: 2GB free space
- **Software**: .NET 8.0 SDK

### Production (Azure App Service)
- **Tier**: Basic B1 or higher recommended
- **OS**: Windows or Linux
- **Runtime**: .NET 8.0

---

## 🎓 Learning Resources

### Microsoft Azure Certifications
- **AZ-900**: Azure Fundamentals
- **AI-900**: Azure AI Fundamentals
- **DP-900**: Azure Data Fundamentals
- **AZ-104**: Azure Administrator
- **AZ-305**: Azure Solutions Architect

### Useful Links
- [Microsoft Learn](https://learn.microsoft.com)
- [Azure Documentation](https://docs.microsoft.com/azure)
- [Gemini API Documentation](https://ai.google.dev/docs)
- [ASP.NET Core Docs](https://docs.microsoft.com/aspnet/core)

---

## 📞 Support

### Getting Help
1. Check the [README.md](README.md) for detailed documentation
2. Review the troubleshooting section above
3. Check application logs in `bin/Debug/net8.0/` folder
4. Contact: support@skilltechclub.com

### Reporting Issues
Please include:
- Error message (if any)
- Steps to reproduce
- Browser/OS information
- Screenshot (if relevant)

---

## ✅ Verification Checklist

Before considering setup complete, verify:

- [ ] .NET 8.0 SDK installed
- [ ] Gemini API key obtained
- [ ] API key configured in appsettings.Development.json
- [ ] Application builds without errors (`dotnet build`)
- [ ] Application runs successfully (`dotnet run`)
- [ ] Browser opens and shows the chat interface
- [ ] Welcome screen displays properly
- [ ] Can send and receive messages
- [ ] Goal badges appear correctly
- [ ] UI is responsive on mobile (test by resizing browser)

---

## 🎉 Next Steps

Now that your SkillTech Navigator is running:

1. **Customize the persona** to match your brand
2. **Add more learning paths** specific to your courses
3. **Implement analytics** to track user interactions
4. **Deploy to production** on Azure
5. **Gather user feedback** and iterate

---

**Congratulations! Your AI Career Mentor is ready to guide learners! 🚀**
