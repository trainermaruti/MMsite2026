# ⚡ QUICK START - SkillTech Copilot

## 🎯 Getting Started in 3 Steps

### Step 1: Get Your API Key (2 minutes)

1. Go to: **https://makersuite.google.com/app/apikey**
2. Sign in with Google
3. Click "Create API Key"
4. Copy the key

---

### Step 2: Configure the App (1 minute)

1. Open: `appsettings.Development.json`
2. Find this line:
   ```json
   "ApiKey": "YOUR_GEMINI_API_KEY_HERE"
   ```
3. Replace with your actual API key:
   ```json
   "ApiKey": "AIzaSyAbCdEfGh..."
   ```
4. Save the file

---

### Step 3: Run the App (30 seconds)

**Option A: Using PowerShell Script (Recommended)**
```powershell
.\start.ps1
```

**Option B: Using Command Line**
```powershell
dotnet run
```

**Option C: Using Visual Studio**
- Press `F5`

---

## 🌐 Access Your App

Open your browser and go to:
- **https://localhost:5001**

---

## 🎯 Test It!

Try these messages:
- "Which certification should I start with?"
- "What is Azure Virtual Network?"
- "I want to pursue a career in cloud computing"
- "Explain the difference between AZ-900 and AI-900"

**Test Exam Refusal:**
- "What is the correct answer to this exam question..."
- (Should refuse and offer to explain concepts instead)

---

## 📚 Need More Help?

- **Full Documentation**: See [README.md](README.md)
- **Testing Guide**: See [TESTING.md](TESTING.md)
- **Deployment**: See [DEPLOYMENT.md](DEPLOYMENT.md)

---

## ❓ Troubleshooting

### "API key not configured" error
→ Check Step 2 above

### Application won't start
→ Run: `dotnet --version` (should be 8.0+)

### Port already in use
→ Stop other applications using ports 5000/5001

---

## 🚀 What Next?

1. ✅ Test locally (see TESTING.md)
2. ✅ Customize the system prompt (edit Services/GeminiService.cs)
3. ✅ Deploy to Azure (see DEPLOYMENT.md)
4. ✅ Share with learners!

---

**That's it! You're ready to go!**
