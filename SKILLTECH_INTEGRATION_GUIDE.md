# 🔗 SkillTech.club Integration Guide

## Overview
This portfolio is now integrated with **SkillTech.club** website, allowing seamless redirection from portfolio cards to actual SkillTech.club courses, trainings, and events.

---

## 🎯 What's Implemented

### 1. **Database Schema Updates**
- Added `SkillTechUrl` field to:
  - `Courses` table
  - `Trainings` table
  - `TrainingEvents` table
- Field Type: `NVARCHAR(500)` (Optional)
- Validation: Must be valid URL format

### 2. **Smart Card Behavior**
Cards now have **two modes**:

#### Mode 1: With SkillTech URL (External Link)
- **Entire card is clickable** and redirects to SkillTech.club
- Shows **green "SkillTech" badge** on card
- Card has **hover lift effect**
- Button text: "View on SkillTech.club →"
- Opens in **new tab** (target="_blank")

#### Mode 2: Without SkillTech URL (Internal Link)
- Card shows local content
- Button: "View Course/Details"
- Links to portfolio detail page

### 3. **Admin Forms Enhanced**
All Create/Edit forms now include:
```html
SkillTech.club URL [Optional]
━━━━━━━━━━━━━━━━━━━━━━━━
https://skilltech.club/courses/...
```
- Optional field with helpful hint text
- URL validation
- Clear explanation of functionality

### 4. **Homepage Enhancements**
Inspired by SkillTech.club website:
- **Trusted Companies Section** - Client logos with hover effects
- **Student Testimonials** - 3 featured reviews with star ratings
- **SkillTech.club CTA** - Prominent call-to-action box linking to SkillTech.club

---

## 📊 Migration Guide

### Step 1: Run SQL Migration
```sql
-- Execute this script to add SkillTechUrl columns
-- File: Migrations/AddSkillTechUrlToAllTables.sql

dotnet ef database update
```

Or manually run the SQL file in SQL Server Management Studio.

### Step 2: Update Existing Records
Navigate to Admin Panel and edit existing courses/trainings/events to add SkillTech URLs.

**Sample SkillTech URLs:**
```
Courses:
https://skilltech.club/courses/azure-fundamentals-certification/az-900/1
https://skilltech.club/courses/azure-ai-fundamentals-certification/ai-900/2
https://skilltech.club/courses/azure-architect-course/az305/6
https://skilltech.club/courses/azure-developer-certification/az-204-certification/11
https://skilltech.club/courses/azure-ai-fundamentals-certification/ai-102-certification/13
https://skilltech.club/courses/azure-administrator-training
https://skilltech.club/courses/microsoft-copilot-studio

Categories:
https://skilltech.club/courses/azure-ai-fundamentals-certification
https://skilltech.club/courses/azure-fundamentals-certification
https://skilltech.club/courses/microsoft-copilot-studio
https://skilltech.club/courses/azure-administrator-training
https://skilltech.club/courses/azure-developer-certification
https://skilltech.club/courses/azure-architect-course
```

---

## 🎨 Visual Indicators

### SkillTech-Linked Cards Show:
1. ✅ Green "SkillTech" badge in top-right
2. 🎯 Entire card is clickable (cursor: pointer)
3. 📤 "View on SkillTech.club →" button text
4. ✨ Hover lift animation
5. 🆕 Opens in new tab

### Regular Cards Show:
1. 📄 "View Course/Details" button
2. 🔗 Links to internal detail page
3. ℹ️ No external link badge

---

## 🚀 Usage Workflow

### For Course Creators:
1. Go to **Admin Panel → Courses → Create/Edit**
2. Fill in all required fields
3. **Optional:** Add SkillTech.club URL in the "SkillTech.club Course URL" field
4. Save

### For Visitors:
1. Visit **/Courses**, **/Trainings**, or **/Events**
2. Cards with **green "SkillTech" badge** will redirect to SkillTech.club when clicked
3. Cards without badge show local portfolio content

---

## 🎯 Content from SkillTech.club Website

### Why Choose SkillTech Club?
- ⏱️ **Learn at Your Pace** - Flexible schedules with self-paced courses
- 🎓 **Microsoft Certified Trainers** - Official Microsoft Learning Partner
- 💸 **Cost-Effective** - Top-tier tech education without breaking the bank
- 🧭 **Choose Your Path** - Wide range of programs in Azure and AI
- 🛠️ **Real-World Skills** - Project-based learning with live projects
- 🚀 **Enhance Your Career** - Boost employability with latest Cloud and AI skills

### Course Categories Available:
1. **Azure AI** - AI-900, AI-102, AI Agent Certification
2. **Azure Fundamentals** - AZ-900 Microsoft Azure Fundamentals
3. **Microsoft Copilot** - Copilot Studio training
4. **Azure Administrator** - AZ-104 certification prep
5. **Azure Developer** - AZ-204 certification
6. **Azure Architect** - AZ-305 Infrastructure Solutions

### Popular Courses on SkillTech.club:
- **AI-900**: Microsoft Azure AI Fundamentals (30 Lessons, 08:08:31)
- **AZ-305**: Azure Architect Solutions (95 Lessons, 05:41:52)
- **AZ-900**: Azure Fundamentals (25 Lessons, 04:55:55)
- **AI Agent Certification**: Intermediate (26 Lessons, 07:59:51)
- **AI-102**: Azure AI Engineer (62 Lessons, 16:45:45)
- **AZ-204**: Azure Developer (59 Lessons, 14:57:14)

---

## 📸 Screenshots & Examples

### Before Integration
```
[Card] → Click → Portfolio Details Page
```

### After Integration (with SkillTech URL)
```
[Card with SkillTech Badge] → Click → SkillTech.club Course Page (New Tab)
```

### Example Card HTML:
```html
<!-- With SkillTech URL -->
<a href="https://skilltech.club/courses/azure-ai/ai-900/2" target="_blank">
  <div class="card hover-lift">
    <span class="badge bg-success">
      <i class="fas fa-external-link-alt"></i> SkillTech
    </span>
    <!-- Card content -->
    <button>View on SkillTech.club →</button>
  </div>
</a>

<!-- Without SkillTech URL -->
<div class="card">
  <!-- Card content -->
  <a href="/Courses/Details/1">View Course</a>
</div>
```

---

## 🔧 Technical Implementation

### Models Updated:
```csharp
// Course.cs, Training.cs, TrainingEvent.cs
[Url(ErrorMessage = "Please enter a valid URL")]
[StringLength(500, ErrorMessage = "SkillTech URL cannot exceed 500 characters")]
public string? SkillTechUrl { get; set; }
```

### View Logic:
```cshtml
@if (!string.IsNullOrEmpty(course.SkillTechUrl))
{
    <!-- External SkillTech Link -->
    <a href="@course.SkillTechUrl" target="_blank">
        <div class="card hover-lift">
            <span class="badge bg-success">SkillTech</span>
            <!-- Card content -->
        </div>
    </a>
}
else
{
    <!-- Internal Portfolio Link -->
    <div class="card">
        <a href="/Courses/Details/@course.Id">View Course</a>
    </div>
}
```

### CSS for Hover Effects:
```css
.hover-lift {
    transition: transform 0.3s ease, box-shadow 0.3s ease;
    cursor: pointer;
}

.hover-lift:hover {
    transform: translateY(-5px);
    box-shadow: 0 10px 25px rgba(0,0,0,0.2) !important;
}
```

---

## 📊 Benefits of Integration

### For Portfolio Visitors:
✅ Seamless access to full SkillTech.club courses  
✅ No manual searching for courses  
✅ Direct enrollment capability  
✅ Access to 100+ lessons and hours of content  

### For Portfolio Owner:
✅ Showcase SkillTech.club content on portfolio  
✅ Drive traffic to main learning platform  
✅ Maintain unified brand presence  
✅ Easy content management (update once on SkillTech, link from portfolio)  

### For Business:
✅ Increased conversion rate (fewer steps to enrollment)  
✅ Better user experience  
✅ Professional brand image  
✅ Analytics tracking across platforms  

---

## 🎓 Testimonials Integration

The homepage now features **real student testimonials** from SkillTech.club:

### Featured Reviews:
1. **Ashek Rasul** - Lead Product Designer  
   ⭐⭐⭐⭐⭐ - "Best trainer I have seen, excellent speaker..."

2. **Shubham Arya** - BI Analyst at IBM  
   ⭐⭐⭐⭐⭐ - "Azure Kubernetes training was worth every minute..."

3. **Sneha K** - BI Analyst at IBM  
   ⭐⭐⭐⭐⭐ - "Hands down the best training session..."

---

## 🏢 Company Logos

Trusted by leading companies (displayed on homepage):
- IBM
- TCS
- Infosys
- Wipro
- Accenture
- Capgemini

Logo effects:
- Grayscale by default
- Full color on hover
- Subtle scale animation

---

## 🔗 Important Links

- **SkillTech.club Homepage**: https://skilltech.club/
- **Courses**: https://skilltech.club/courses
- **Blog**: https://skilltech.club/blog
- **Membership**: https://skilltech.club/home/pricing
- **About Maruti Makwana**: https://skilltech.club/marutimakwana

### Social Media:
- YouTube: https://www.youtube.com/@skilltechclub
- Instagram: https://www.instagram.com/skilltech.club
- Facebook: https://www.facebook.com/skilltechclub
- LinkedIn: https://www.linkedin.com/company/skilltechclub

---

## 📝 Next Steps

### Immediate Actions:
1. ✅ Run database migration
2. ✅ Test card behavior (with and without SkillTech URL)
3. ✅ Update existing course records with SkillTech URLs
4. ✅ Verify links open in new tabs
5. ✅ Test responsive design on mobile

### Optional Enhancements:
- [ ] Add analytics tracking for SkillTech link clicks
- [ ] Create admin dashboard showing click-through rates
- [ ] Add "Popular on SkillTech" section on homepage
- [ ] Sync course thumbnails from SkillTech.club API
- [ ] Implement lazy loading for company logos
- [ ] Add more testimonials rotation/carousel

---

## 🐛 Troubleshooting

### Issue: Cards not linking to SkillTech
**Solution:** Ensure `SkillTechUrl` field is populated in database and not empty string

### Issue: Links opening in same tab
**Solution:** Verify `target="_blank"` attribute in view files

### Issue: Hover effect not working
**Solution:** Check `.hover-lift` CSS class is included and not overridden

### Issue: Badge not showing
**Solution:** Verify `!string.IsNullOrEmpty(course.SkillTechUrl)` condition in view

---

## 📞 Support

For questions or issues:
- Check migration logs: `Migrations/AddSkillTechUrlToAllTables.sql`
- Review view files: `Views/Courses/Index.cshtml`, `Views/Trainings/Index.cshtml`, `Views/Events/Index.cshtml`
- Test with sample SkillTech URLs provided above

---

**Last Updated:** December 1, 2025  
**Version:** 1.4 - SkillTech Integration  
**Status:** ✅ Ready for Production
