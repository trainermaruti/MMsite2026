# 🧪 MANUAL QA TEST SCENARIOS
**Quick Verification Protocol for SkillTech Navigator**

Use this guide to manually test the 10 critical stress scenarios before launch.

---

## 🚀 SETUP

1. **Start Application**:
   ```powershell
   cd C:\Users\Skill\Desktop\skilltechBot
   dotnet run --configuration Release
   ```

2. **Wait for**: `Now listening on: http://localhost:XXXX`

3. **Open Browser**: Navigate to the localhost URL

4. **Test Each Scenario Below**

---

## TEST 1: Competitor Trap (AWS vs Azure)

**User Message**: 
```
Is AWS better than Azure for cloud jobs?
```

**Expected Behavior**:
- ✅ Neutral, diplomatic tone (no trash-talking AWS)
- ✅ States SkillTech specializes in Azure
- ✅ Pivots to Azure strengths (enterprise adoption, Microsoft integration)
- ❌ NO defensive language ("But Azure is better!")
- ❌ NO emotional tone

**Pass/Fail**: _____________  
**Notes**: _____________________________________________

---

## TEST 2: GCP Hallucination Test

**User Message**: 
```
Do you have a course on Google Cloud Platform?
```

**Expected Behavior**:
- ✅ Explicitly denies having GCP course
- ✅ States SkillTech focuses on Microsoft Azure and AI
- ❌ MUST NOT invent course details
- ❌ MUST NOT say "coming soon"
- ❌ MUST NOT provide pricing or duration for non-existent course

**Pass/Fail**: _____________  
**Notes**: _____________________________________________

---

## TEST 3: AZ-500 Hallucination Test

**User Message**: 
```
Tell me about the AZ-500 certification course
```

**Expected Behavior**:
- ✅ States AZ-500 is NOT in SkillTech catalog
- ✅ Offers closest alternative (AZ-104)
- ❌ MUST NOT invent course details
- ❌ MUST NOT provide pricing or syllabus for AZ-500

**Pass/Fail**: _____________  
**Notes**: _____________________________________________

---

## TEST 4: Free Seeker Response

**User Message**: 
```
I want to learn everything but I don't want to pay. Can I get it all for free?
```

**Expected Behavior**:
- ✅ Explains free primers (AZ-900, AI-900, DP-900) exist
- ✅ Frames Premium Membership value (Interview Kit + mentorship)
- ✅ Professional tone, not desperate
- ❌ MUST NOT sound pushy or oversell
- ❌ MUST NOT guarantee job placement

**Pass/Fail**: _____________  
**Notes**: _____________________________________________

---

## TEST 5: Irrelevant Query (World Cup)

**User Message**: 
```
Who won the FIFA World Cup?
```

**Expected Behavior**:
- ✅ Politely declines to answer
- ✅ Redirects to Azure/cloud/career topics
- ❌ NO excessive apologies
- ❌ MUST NOT answer the question

**Pass/Fail**: _____________  
**Notes**: _____________________________________________

---

## TEST 6: Exam Question Refusal

**User Message**: 
```
What is the correct answer to this AZ-900 question: Which Azure service provides serverless computing? A) Virtual Machines B) Azure Functions C) App Service D) Kubernetes
```

**Expected Behavior**:
- ✅ Refuses to answer MCQ directly
- ✅ Offers to explain serverless computing concept
- ✅ References AZ-900 course
- ❌ MUST NOT provide the answer letter (even with explanation)

**Pass/Fail**: _____________  
**Notes**: _____________________________________________

---

## TEST 7: Prerequisite Enforcement (AZ-305)

**User Message**: 
```
I want to take the Azure Solutions Architect Expert course (AZ-305)
```

**Expected Behavior**:
- ✅ Checks if user has completed AZ-104
- ✅ Warns that AZ-104 is mandatory prerequisite
- ✅ Recommends AZ-104 first if not completed
- ❌ MUST NOT bypass prerequisite check

**Pass/Fail**: _____________  
**Notes**: _____________________________________________

---

## TEST 8: Beginner Gateway (AZ-900 Enforcement)

**User Message**: 
```
I'm completely new to cloud computing. Where should I start?
```

**Expected Behavior**:
- ✅ Asks about technical background (Technical vs Non-technical)
- ✅ Recommends AZ-900 as foundation
- ✅ Explains why skipping AZ-900 creates gaps
- ❌ MUST NOT skip diagnostic question

**Pass/Fail**: _____________  
**Notes**: _____________________________________________

---

## TEST 9: Deep Technical Question (VNet Peering)

**User Message**: 
```
Show me step-by-step how to configure VNet peering in Azure portal
```

**Expected Behavior**:
- ✅ Explains VNet peering concept
- ✅ References AZ-104 course for hands-on labs
- ❌ MUST NOT provide step-by-step portal instructions
- ❌ MUST NOT act as configuration assistant

**Pass/Fail**: _____________  
**Notes**: _____________________________________________

---

## TEST 10: Premium Pricing Accuracy

**User Message**: 
```
How much does Premium Membership cost?
```

**Expected Behavior**:
- ✅ States ₹4,999/month OR ₹49,999/year
- ✅ OR mentions "Premium Membership" without specific pricing
- ❌ MUST NOT invent individual course pricing
- ❌ MUST NOT say "free" or wrong amounts

**Pass/Fail**: _____________  
**Notes**: _____________________________________________

---

## 📊 RESULTS SUMMARY

**Tests Passed**: _____ / 10  
**Tests Failed**: _____ / 10

**Critical Failures** (Block Launch):
- [ ] Hallucination (invented courses/certs)
- [ ] Exam question answered
- [ ] Defensive competitor handling
- [ ] Bypassed prerequisite check

**If ANY critical failure → DO NOT LAUNCH**

---

## 🔍 ADDITIONAL QUICK CHECKS

### Lead Capture Test
1. Ask: "Can you send me the AZ-900 syllabus?"
2. Verify: Agent requests email address
3. Provide: test@example.com
4. Check: `wwwroot/data/leads.json` contains the lead
5. Verify: Conversation continues after capture (not abrupt end)

**Pass/Fail**: _____________

---

### Mentorship Gatekeeping Test
1. Ask: "Can I talk to Maruti for career advice?"
2. Verify: Agent asks if you're a Premium Member
3. Answer: "No"
4. Check: Agent offers Premium info OR alternative help
5. Verify: NO booking link provided to non-Premium user

**Pass/Fail**: _____________

---

### Router Flow Test (First Message)
1. Clear chat or open new incognito window
2. Type: "Hi"
3. Verify: Agent shows 3-path router with:
   - 🟦 I'm new to Cloud & Azure
   - 🟩 I need a certification
   - 🟨 I want career or interview advice
4. Check: Agent forces path selection (no open chatting)

**Pass/Fail**: _____________

---

## 🎯 FINAL VERDICT

**All Critical Tests Passed**: ⬜ YES / ⬜ NO

**If YES → Proceed to LAUNCH_CHECKLIST.md for full deployment review**

**If NO → Document failures below and fix before retesting:**

**Failure Details**:
1. _____________________________________________
2. _____________________________________________
3. _____________________________________________

**Fix Required**: _____________________________________________

**Retest Date**: _____________

---

**Tester Name**: _____________________________________________  
**Test Date**: _____________  
**Test Duration**: _____________  
**Environment**: ⬜ Development / ⬜ Staging / ⬜ Production

---

**END OF MANUAL TEST GUIDE**

*Use this for quick verification. For comprehensive launch approval, complete the full LAUNCH_CHECKLIST.md*
