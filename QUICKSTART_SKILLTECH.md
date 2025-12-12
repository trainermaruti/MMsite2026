# 🚀 SkillTech Integration - Quick Start Guide

## ⚡ Get Started in 3 Steps

### Step 1: Run Database Migration (2 minutes)

Open PowerShell in your project directory and run:

```powershell
# Execute the SQL migration
dotnet ef database update
```

**Alternative:** Open SQL Server Management Studio and run the SQL file:
- File: `Migrations/AddSkillTechUrlToAllTables.sql`

---

### Step 2: Test the Integration (5 minutes)

```powershell
# Start the application
dotnet run
```

Open browser and navigate to:
- **Homepage**: https://localhost:5204/
- **Courses**: https://localhost:5204/Courses
- **Trainings**: https://localhost:5204/Trainings
- **Events**: https://localhost:5204/Events

**What to look for:**
- ✅ Homepage shows "Trusted Companies" section with 6 logos
- ✅ Homepage shows "Student Testimonials" section with 3 reviews
- ✅ Homepage shows purple "SkillTech.club CTA" box
- ✅ All pages load without errors

---

### Step 3: Add SkillTech URLs to Courses (10 minutes)

1. **Login to Admin Panel:**
   - Navigate to: https://localhost:5204/Admin/Dashboard
   - Username: `admin@marutitraining.com`
   - Password: `Admin@123456`

2. **Edit a Course:**
   - Click "Manage Courses"
   - Click "Edit" on any course
   - Scroll to "SkillTech.club Course URL" field
   - Paste a URL from the list below
   - Click "Update Course"

3. **Sample URLs to use:**

```
Azure Fundamentals AZ-900:
https://skilltech.club/courses/azure-fundamentals-certification/az-900/1

Azure AI Fundamentals AI-900:
https://skilltech.club/courses/azure-ai-fundamentals-certification/ai-900/2

Azure Architect AZ-305:
https://skilltech.club/courses/azure-architect-course/az305/6

Azure Developer AZ-204:
https://skilltech.club/courses/azure-developer-certification/az-204-certification/11

AI-102 Certification:
https://skilltech.club/courses/azure-ai-fundamentals-certification/ai-102-certification/13

Azure Administrator:
https://skilltech.club/courses/azure-administrator-training

Microsoft Copilot:
https://skilltech.club/courses/microsoft-copilot-studio
```

4. **Verify the Changes:**
   - Go back to: https://localhost:5204/Courses
   - You should see a **green "SkillTech" badge** on the card you edited
   - Click anywhere on that card
   - It should open SkillTech.club in a **new tab**

---

## 🎯 Visual Guide

### Before Adding SkillTech URL:
```
┌─────────────────────────────┐
│ Azure Fundamentals          │
│ Learn the basics of Azure   │
│                             │
│ [View Course]               │ ← Internal link
└─────────────────────────────┘
```

### After Adding SkillTech URL:
```
┌─────────────────────────────┐
│ Azure Fundamentals     [🟢] │ ← SkillTech badge
│ Learn the basics of Azure   │
│                             │
│ [View on SkillTech.club →]  │ ← External link
└─────────────────────────────┘
↑ Entire card clickable
↑ Hover shows lift animation
```

---

## ✅ Success Checklist

After completing the 3 steps, verify:

- [ ] Database migration executed successfully (no errors in console)
- [ ] Homepage shows "Trusted Companies" section
- [ ] Homepage shows "Student Testimonials" section  
- [ ] Homepage shows "SkillTech.club CTA" purple box
- [ ] At least one course has SkillTech URL added
- [ ] Course with SkillTech URL shows green "SkillTech" badge
- [ ] Clicking SkillTech card opens SkillTech.club in new tab
- [ ] Course without SkillTech URL shows normal "View Course" button
- [ ] All pages responsive on mobile (resize browser window)

---

## 🔥 Pro Tips

### Tip 1: Bulk Update URLs
If you have many courses, update them in batches:
1. Open Admin Panel
2. Click "Manage Courses"
3. Edit courses one by one
4. Use the sample URLs provided above

### Tip 2: Test Both Modes
Create two test courses:
- **Course A**: WITH SkillTech URL → Should redirect externally
- **Course B**: WITHOUT SkillTech URL → Should show local content

### Tip 3: Mobile Testing
```powershell
# Check responsive design
# Press F12 in browser → Toggle device toolbar
# Test on iPhone, iPad, Desktop views
```

### Tip 4: Clear Browser Cache
If changes don't appear immediately:
- Press `Ctrl + Shift + R` (Windows) or `Cmd + Shift + R` (Mac)
- Or press `Ctrl + F5` for hard refresh

---

## 📊 Expected Results

### Homepage Changes:
- **New Section 1**: Trusted Companies (6 logos)
- **New Section 2**: Student Testimonials (3 cards)
- **New Section 3**: SkillTech CTA (purple gradient box)

### Courses Page:
- Cards **with** SkillTech URL:
  - Show green "SkillTech" badge
  - Entire card clickable
  - Redirects to SkillTech.club
  - Opens in new tab
  
- Cards **without** SkillTech URL:
  - No badge
  - "View Course" button
  - Opens local details page

---

## 🐛 Troubleshooting

### Problem: Migration fails
**Solution:**
```powershell
# Check connection string in appsettings.json
# Ensure SQL Server LocalDB is running
# Try: dotnet ef database drop (WARNING: Deletes all data!)
# Then: dotnet ef database update
```

### Problem: Cards not showing SkillTech badge
**Solution:**
- Verify you added the URL in admin panel
- Check the URL field is not empty
- Refresh browser with Ctrl+F5
- Check browser console for errors (F12)

### Problem: Homepage sections missing
**Solution:**
- Ensure you pulled latest code
- Check Views/Home/Index.cshtml file exists
- Verify CSS file is loaded (check browser network tab)

### Problem: Clicking card does nothing
**Solution:**
- Check browser console for JavaScript errors
- Verify the SkillTech URL is valid (starts with https://)
- Test in different browser (Chrome, Edge, Firefox)

---

## 📞 Need Help?

### Documentation Files:
- **Full Guide**: `SKILLTECH_INTEGRATION_GUIDE.md`
- **Implementation Summary**: `IMPLEMENTATION_SUMMARY_SKILLTECH.md`
- **Website Info**: `WEBSITE_INFORMATION.txt`
- **Migration Script**: `Migrations/AddSkillTechUrlToAllTables.sql`

### Quick Commands:
```powershell
# View database migrations
dotnet ef migrations list

# Remove last migration (if needed)
dotnet ef migrations remove

# Check EF Core version
dotnet ef --version

# Restore packages
dotnet restore

# Clean and rebuild
dotnet clean
dotnet build
```

---

## 🎉 You're Done!

Congratulations! Your portfolio is now integrated with SkillTech.club.

### What You Achieved:
✅ Smart card redirection system  
✅ Visual SkillTech badges  
✅ Professional testimonials section  
✅ Company logos showcase  
✅ SkillTech.club call-to-action  
✅ Seamless user experience  

### Next Actions:
1. ✅ Share the portfolio URL with colleagues
2. ✅ Monitor click-through rates to SkillTech.club
3. ✅ Add more courses with SkillTech URLs
4. ✅ Gather visitor feedback
5. ✅ Deploy to production when ready

---

## 🚀 Go Live Checklist

When ready for production:

- [ ] Change default admin password
- [ ] Update connection string to production database
- [ ] Test all SkillTech URLs are correct
- [ ] Verify SSL certificate is configured
- [ ] Test on real mobile devices
- [ ] Run performance tests
- [ ] Set up Google Analytics (optional)
- [ ] Configure backup strategy
- [ ] Monitor error logs
- [ ] Announce launch on social media

---

**Happy Linking! 🎓**

Your portfolio and SkillTech.club are now working together seamlessly!

---

*Last Updated: December 1, 2025*  
*Quick Start Version: 1.0*  
*Status: ✅ Production Ready*
