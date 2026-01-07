# 🚀 VOICEFLOW DEPLOYMENT GUIDE
## SkillTech Navigator — Production Implementation

**Platform**: Voiceflow  
**Deployment Type**: Web Chat Widget  
**Last Updated**: January 3, 2026  
**Status**: Production-Ready

---

## 📋 PREREQUISITES

Before starting, ensure you have:

- ✅ Voiceflow account (Pro or Team plan recommended)
- ✅ SkillTech.club website access (WordPress/HTML)
- ✅ SkillTech_KnowledgeBase.txt file
- ✅ full_course_catalog.json file
- ✅ SYSTEM_PROMPT.txt file
- ✅ SkillTech Club logo file (for avatar)

---

## STEP 1: PLATFORM SETUP (VOICEFLOW)

### 1.1 Create the Project

1. Log in to [Voiceflow](https://www.voiceflow.com/)
2. Click **"Create New Project"**
3. Select **"Build AI Assistant"**
4. Choose **"Web Chat"** (NOT "Experimental" or "Quick Chat")
5. Name project: **"SkillTech Navigator"**

**⚠️ CRITICAL**: Use Web Chat for full control. Experimental modes limit button functionality.

---

### 1.2 Upload the Knowledge Base

1. Navigate to: **Knowledge Base → Data Sources**
2. Click **"Add Data Source"**
3. Upload **TWO** files:
   - `SkillTech_KnowledgeBase.txt`
   - `full_course_catalog.json`

**📌 RULES**:
- ✅ JSON must remain **unedited**
- ✅ Do NOT merge files
- ✅ Do NOT rely on web scraping
- ❌ Missing JSON = AI hallucinations

**Verification**: Confirm both files show as "Active" in Data Sources panel.

---

### 1.3 Lock the Persona (CRITICAL)

1. Go to: **Settings → AI Persona**
2. Open `SYSTEM_PROMPT.txt` from deployment files
3. **Copy the ENTIRE prompt** (5000+ characters)
4. Paste into **"System Instructions"** field
5. Click **Save**

**⚠️ NON-NEGOTIABLE RULES**:
- ❌ Do NOT shorten the prompt
- ❌ Do NOT "simplify" for friendliness
- ❌ Do NOT remove sections
- ✅ This prompt is your guardrail against hallucinations

**Verification**: Re-open AI Persona and confirm full prompt is visible.

---

### 1.4 Connect the Router Flow

1. Open **Visual Builder** (Canvas)
2. Delete the default welcome block
3. Create **Entry Block** with trigger: `User Message: "hi", "hello", "hey"`
4. Add **Response Block** with text:

```
Hi, I'm the SkillTech Navigator.
I help people choose the right Azure certification, plan their cloud career, or book 1-to-1 mentorship with Maruti Makwana.

What are you here for today?
```

5. Add **3 Button Choices**:
   - 🟦 I'm new to Cloud & Azure → `FLOW_BEGINNER`
   - 🟩 I need a certification → `FLOW_COURSE_SELECTOR`
   - 🟨 I want career or interview advice → `FLOW_MENTORSHIP`

**📌 CRITICAL**: Button-based routing is MANDATORY. No free-text options at this stage.

---

### 1.5 Build the Three Core Flows

#### **FLOW A: BEGINNER PATH**

Block 1: Background Check
```
Got it. Since you're new, the most important thing is building the right foundation.

Before we go further, tell me — do you come from a technical background?
```

Buttons:
- Technical / IT / Engineering → Block 2a
- Non-technical / Career switcher → Block 2b

Block 2a (Technical):
```
Good. Even with a technical background, AZ-900 is still the correct starting point for Azure.

Once that's done, you'll move faster in role-based certifications.
```
[Add AZ-900 card with: Duration, Free status, URL]

Block 2b (Non-Technical):
```
Perfect. You should start with AZ-900 (Azure Fundamentals).

It explains cloud concepts, pricing, security, and Azure services without coding.
Skipping this causes confusion later.
```
[Add AZ-900 card]

---

#### **FLOW B: COURSE SELECTOR**

Block 1: Diagnostic Question
```
To recommend the correct certification, I need one thing:

Are you more interested in writing code or managing cloud infrastructure?
```

Buttons:
- 👨‍💻 Write code (Developer) → Block 2a
- 🛠️ Manage systems (Admin / Ops) → Block 2b
- 🤖 Work with AI → Block 2c

Block 2a (Developer Fork):
```
For developers, the standard Azure path is AZ-204 — it focuses on building cloud-native applications.

However, many developers today are shifting toward AI.

Do you want to build traditional cloud apps or AI-powered systems?
```
Buttons:
- Cloud apps → [Show AZ-204 details]
- AI systems → [Show AI-102 details]

Block 2b (Admin Path):
```
For infrastructure and operations roles, AZ-104 is the industry standard.
It covers identity, networking, compute, and storage — the backbone of Azure.

Important: Without AZ-104, Architect-level certifications are not practical.
```
[Show AZ-104 details]

Block 2c (AI Path):
```
Good choice. Let's clarify one thing first:

Do you want to understand AI concepts or build AI solutions?
```
Buttons:
- Understand AI → [Show AI-900]
- Build AI → [Show AI-102]
- Build copilots (low-code) → [Show Copilot Studio]

---

#### **FLOW C: MENTORSHIP HANDOFF**

Block 1: Premium Check
```
Maruti provides 1-to-1 mentorship calls for Premium Members.
These sessions focus on career mapping, certification strategy, and interview preparation.

Are you currently a Premium Member?
```

Buttons:
- Yes → Block 2a
- No → Block 2b

Block 2a (Premium Member):
```
Great. Here's the private booking link for mentorship calls:
https://skilltech.club/mentorship
```

Block 2b (Non-Premium):
```
No problem.

Premium Membership unlocks mentorship, interview kits, and full course access.
Or, if you prefer, I can answer some questions here first.
```
Buttons:
- View Premium → [Link to premium page]
- Ask questions here → [Return to main router]

---

## STEP 2: WIDGET BRANDING

### 2.1 Configure Widget Settings

1. Go to: **Settings → Widget → Appearance**
2. Configure:

| Setting | Value |
|---------|-------|
| **Agent Name** | SkillTech Navigator |
| **Avatar** | Upload SkillTech Club logo OR use "ST" text avatar |
| **Primary Color** | `#0078D4` (Azure Blue) |
| **Font** | Default (Inter/System) |
| **Widget Position** | Bottom-right |
| **Widget Size** | Medium |

---

### 2.2 Welcome Message

In Widget Settings → **Launcher**, set:

**Welcome Text**:
```
Hi, I'm the SkillTech Navigator.
I help you choose the right Azure certification or plan your career with expert mentorship.

What would you like to work on today?
```

**⚠️ RULES**:
- ❌ No emojis
- ❌ No hype language
- ✅ Professional education platform tone

---

### 2.3 Widget Behavior

Enable:
- ✅ **Persistent chat** (saves conversation history)
- ✅ **Typing indicators**
- ✅ **Avatar display**
- ❌ **Sound notifications** (disable for professional setting)

---

## STEP 3: EMBED CODE GENERATION

### 3.1 Generate the Script

1. Click **"Publish"** in Voiceflow
2. Select **"Production"** environment
3. Copy the generated embed code

**Example Script** (your actual script will have a unique Project ID):

```html
<script type="text/javascript">
  (function(d, t) {
      var v = d.createElement(t), s = d.getElementsByTagName(t)[0];
      v.onload = function() {
        window.voiceflow.chat.load({
          verify: { projectID: 'YOUR_PROJECT_ID_HERE' },
          url: 'https://general-runtime.voiceflow.com',
          versionID: 'production'
        });
      };
      v.src = "https://cdn.voiceflow.com/widget/bundle.mjs";
      v.type = "text/javascript";
      s.parentNode.insertBefore(v, s);
  })(document, 'script');
</script>
```

**⚠️ CRITICAL RULES**:
- ❌ Do NOT rename variables
- ❌ Do NOT inline into React components (wrap properly)
- ❌ Do NOT minify manually
- ✅ Use script EXACTLY as provided

---

## STEP 4: INSTALLATION ON SKILLTECH.CLUB

### Option A: WordPress Installation (MOST COMMON)

#### Method 1: WPCode Plugin (Recommended)

1. **Install Plugin**:
   - Go to: `Plugins → Add New`
   - Search: **"WPCode"**
   - Install & Activate

2. **Add Script**:
   - Go to: `Code Snippets → Header & Footer`
   - Paste script into: **"Footer"** section (NOT Header)
   - Location: Before `</body>`
   - Click: **Save Changes**

3. **Verify**:
   - Visit any page on SkillTech.club
   - Widget should appear bottom-right

#### Method 2: Theme Footer (Alternative)

1. Go to: `Appearance → Theme File Editor`
2. Open: `footer.php`
3. Find: `</body>` tag
4. Paste script **immediately before** `</body>`
5. Click: **Update File**

**⚠️ WARNING**: Backup theme before editing files directly.

---

### Option B: Custom HTML / React / Webflow

#### Static HTML:
```html
<!DOCTYPE html>
<html>
<head>
    <title>SkillTech Club</title>
</head>
<body>
    <!-- Your content -->
    
    <!-- Voiceflow Widget (Before closing body tag) -->
    <script type="text/javascript">
      (function(d, t) {
          var v = d.createElement(t), s = d.getElementsByTagName(t)[0];
          v.onload = function() {
            window.voiceflow.chat.load({
              verify: { projectID: 'YOUR_PROJECT_ID_HERE' },
              url: 'https://general-runtime.voiceflow.com',
              versionID: 'production'
            });
          };
          v.src = "https://cdn.voiceflow.com/widget/bundle.mjs";
          v.type = "text/javascript";
          s.parentNode.insertBefore(v, s);
      })(document, 'script');
    </script>
</body>
</html>
```

#### React Integration:
```jsx
import { useEffect } from 'react';

function App() {
  useEffect(() => {
    const script = document.createElement('script');
    script.type = 'text/javascript';
    script.onload = function() {
      window.voiceflow.chat.load({
        verify: { projectID: 'YOUR_PROJECT_ID_HERE' },
        url: 'https://general-runtime.voiceflow.com',
        versionID: 'production'
      });
    };
    script.src = 'https://cdn.voiceflow.com/widget/bundle.mjs';
    document.body.appendChild(script);
  }, []);

  return <div>Your App</div>;
}
```

---

## STEP 5: ADVANCED SETTINGS (CONVERSION OPTIMIZATION)

### 5.1 Enable Proactive Messaging

1. Go to: **Widget Settings → Proactive Messages**
2. Click: **Enable**
3. Configure:

| Setting | Value |
|---------|-------|
| **Trigger** | Time on Page |
| **Delay** | 10 seconds |
| **Message** | "Quick check — are you exploring the Beginner Azure path (AZ-900) or aiming for an advanced role like Architect?" |
| **Show Once** | Per session |

**Why This Works**:
- ✅ Binary choice (easy decision)
- ✅ Career-focused (high intent)
- ✅ Immediately routes into strongest flows
- 📈 Can lift engagement 30-50% for qualified traffic

---

### 5.2 Lead Capture Configuration

1. In Voiceflow, create **Lead Capture Block**
2. Trigger: User mentions "syllabus", "pricing", "demo", "download"
3. Prompt: "What email address should I send it to?"
4. Store: Save email to `{email}` variable
5. API Integration: Send to SkillTech CRM or database

**Post-Capture Action**:
```
Done. I've sent it to your inbox.

While you're here — do you want help understanding the exam format or career impact of this certification?
```

---

### 5.3 Analytics Tracking

1. Go to: **Settings → Integrations**
2. Add **Google Analytics** (if using GA4):
   - Measurement ID: `G-XXXXXXXXXX`
   - Track: Conversations started, Buttons clicked, Flows completed

3. Add **Custom Events**:
   - `lead_captured` (when email collected)
   - `course_selected` (which certification chosen)
   - `mentorship_requested` (premium interest)

---

## STEP 6: TESTING PROTOCOL

### 6.1 Pre-Launch Checklist

Run ALL tests before going live:

**✅ Widget Load Test**:
- [ ] Widget appears bottom-right
- [ ] Avatar displays correctly
- [ ] Colors match Azure branding (#0078D4)
- [ ] No console errors

**✅ Router Flow Test**:
- [ ] Greeting triggers 3-path choice
- [ ] Buttons work (not clickable links)
- [ ] No free-text escape routes

**✅ Beginner Path Test**:
- [ ] Technical/Non-technical fork works
- [ ] AZ-900 always recommended
- [ ] Course details display (duration, URL, free status)

**✅ Course Selector Test**:
- [ ] Developer → AZ-204 or AI-102
- [ ] Admin → AZ-104
- [ ] AI → AI-900, AI-102, or Copilot Studio
- [ ] No hallucinated courses

**✅ Mentorship Test**:
- [ ] Premium check works
- [ ] Mentorship URL displays for premium users
- [ ] Upsell message for non-premium users

**✅ Lead Capture Test**:
- [ ] Email prompt appears when user asks for syllabus
- [ ] Email stored correctly
- [ ] Conversation continues post-capture

**✅ Academic Integrity Test**:
- [ ] Refuses to answer exam MCQs
- [ ] Teaches concepts instead of shortcuts
- [ ] No exam dumps or braindumps

---

### 6.2 Live Testing Script

**Test 1: Beginner**
```
User: "Hi"
Expected: Router with 3 buttons
User: [Click "I'm new to Cloud & Azure"]
Expected: Background question (Technical/Non-technical)
User: "Non-technical"
Expected: AZ-900 recommendation with details
```

**Test 2: AI Path**
```
User: "I want to learn AI"
Expected: AI path disambiguation
User: "Build AI solutions"
Expected: AI-102 recommendation
Verify: Prerequisites shown (AI-900, AZ-900)
```

**Test 3: Architect Gatekeeping**
```
User: "I want to become an Azure Architect"
Expected: Question about AZ-104 completion
User: "No, I haven't done AZ-104"
Expected: "AZ-104 is mandatory..." warning
```

**Test 4: Exam Refusal**
```
User: "What is the correct answer for AZ-900 question 5?"
Expected: Refusal + concept teaching offer
Verify: No direct answer provided
```

---

## STEP 7: GO-LIVE CHECKLIST

Before announcing to users:

**Final Verification**:
- [ ] All flows tested successfully
- [ ] Knowledge Base files active
- [ ] System prompt locked and saved
- [ ] Widget branding matches SkillTech brand
- [ ] Proactive message enabled (10s delay)
- [ ] Analytics tracking configured
- [ ] Lead capture functional
- [ ] No console errors on production site
- [ ] Mobile responsive (test on phone)
- [ ] Loads within 3 seconds

**Post-Launch Monitoring**:
- [ ] Check analytics daily for first week
- [ ] Monitor conversation logs for hallucinations
- [ ] Track lead capture rate
- [ ] Measure Premium conversion rate
- [ ] Collect user feedback

---

## 🚨 TROUBLESHOOTING

### Widget Not Appearing
- ✅ Check script is before `</body>` tag
- ✅ Verify Project ID is correct
- ✅ Clear browser cache
- ✅ Check browser console for errors

### AI Hallucinating Courses
- ✅ Verify JSON file uploaded correctly
- ✅ Re-upload Knowledge Base files
- ✅ Check System Prompt is complete
- ✅ Rebuild flows in Visual Builder

### Buttons Not Working
- ✅ Ensure using Web Chat (not Experimental)
- ✅ Verify button syntax in Voiceflow
- ✅ Check flow connections in Visual Builder

### Lead Capture Not Saving
- ✅ Verify email variable is created
- ✅ Check API integration setup
- ✅ Test with valid email format

### Proactive Message Not Showing
- ✅ Enable in Widget Settings
- ✅ Verify 10-second delay setting
- ✅ Clear cookies and test in incognito

---

## 📊 SUCCESS METRICS

Monitor these KPIs:

**Engagement Metrics**:
- Conversations started per day
- Average messages per conversation
- Flow completion rate (how many reach course selection)

**Conversion Metrics**:
- Email capture rate (% of conversations)
- Premium membership inquiries
- Mentorship booking requests

**Quality Metrics**:
- Academic integrity compliance (exam refusals)
- AZ-900 enforcement rate for beginners
- Prerequisite adherence (AZ-104 before AZ-305)

**Target Benchmarks**:
- Lead capture rate: 30-40% of qualified conversations
- Premium interest: 15-20% of conversations
- Flow completion: 70-80% reach course selection

---

## 📞 SUPPORT & MAINTENANCE

**Voiceflow Support**: support@voiceflow.com  
**SkillTech Technical Contact**: support@skilltech.club  
**Knowledge Base Updates**: Update files in Voiceflow Data Sources monthly

**Recommended Maintenance**:
- Weekly: Review conversation logs
- Monthly: Update course pricing/URLs
- Quarterly: Refine flows based on analytics

---

## ✅ DEPLOYMENT COMPLETE

Once all steps are verified:

1. ✅ Widget live on SkillTech.club
2. ✅ All 3 flows tested and working
3. ✅ Lead capture functional
4. ✅ Analytics tracking active
5. ✅ Proactive messaging enabled

**Your SkillTech Navigator is now production-ready.**

**Remember**: This is not a chatbot. This is a career advisor, sales qualifier, and brand representative. Treat it accordingly.

---

**Last Updated**: January 3, 2026  
**Version**: 1.0 (Production)  
**Maintained By**: SkillTech Development Team
