# SkillTech Navigator - Testing Guide

## 🧪 Manual Testing Procedures

### Pre-Testing Checklist

Before you begin testing, ensure:
- [ ] .NET 8.0 SDK is installed
- [ ] Gemini API key is configured in `appsettings.Development.json`
- [ ] Application builds successfully (`dotnet build`)
- [ ] Application is running (`dotnet run`)

---

## Test Suite 1: Persona Verification

### Test 1.1: Agent Identity
**Objective**: Verify the agent introduces itself as Maruti Makwana

**Steps**:
1. Open the application in your browser
2. Type: "Who are you?"
3. Send the message

**Expected Result**:
- Response mentions "Maruti Makwana"
- Response mentions "SkillTech Navigator" or "Digital Mentor"
- Response references Azure/AI expertise

**Pass Criteria**: ✅ All three elements present

---

### Test 1.2: Tone and Personality
**Objective**: Verify warm, encouraging, professional tone

**Steps**:
1. Type: "I'm not sure if I can learn Azure"
2. Send the message

**Expected Result**:
- Encouraging response
- Addresses concerns positively
- Offers specific guidance or resources

**Pass Criteria**: ✅ Response is supportive and actionable

---

## Test Suite 2: Goal-Based Routing

### Test 2.1: Course Recommendations Goal
**Objective**: Verify "Course Recommendations" goal detection

**Test Messages**:
- "I want to learn Azure"
- "Which course should I start with?"
- "I'm a beginner in cloud computing"
- "Tell me about AI certifications"

**Steps for Each**:
1. Type the message
2. Send and wait for response
3. Check the goal badge (💡 indicator)

**Expected Result**:
- Goal badge shows: "💡 Course Recommendations"
- Response includes specific course suggestions
- May mention certifications (AZ-900, AI-900, etc.)

**Pass Criteria**: ✅ Goal badge correct AND relevant course info provided

---

### Test 2.2: Sales Goal
**Objective**: Verify "Sales" goal detection

**Test Messages**:
- "How much does it cost?"
- "Is this free?"
- "What's the pricing?"
- "Can I buy a premium plan?"

**Steps for Each**:
1. Type the message
2. Send and wait for response
3. Check the goal badge

**Expected Result**:
- Goal badge shows: "💡 Sales"
- Response discusses value proposition
- May mention pricing, ROI, or benefits

**Pass Criteria**: ✅ Goal badge correct AND sales-focused response

---

### Test 2.3: Support Goal
**Objective**: Verify "Support" goal detection

**Test Messages**:
- "I need help"
- "I'm having a problem"
- "Something is not working"
- "Can you support me?"

**Steps for Each**:
1. Type the message
2. Send and wait for response
3. Check the goal badge

**Expected Result**:
- Goal badge shows: "💡 Support"
- Response offers assistance
- May ask clarifying questions

**Pass Criteria**: ✅ Goal badge correct AND helpful response

---

### Test 2.4: General Conversation
**Objective**: Verify handling of general queries

**Test Messages**:
- "Hello"
- "Tell me about yourself"
- "What's the weather like?"

**Steps for Each**:
1. Type the message
2. Send and wait for response
3. Check the goal badge

**Expected Result**:
- Goal badge shows: "💡 General" (or no specific goal badge)
- Response is conversational
- Agent stays in character

**Pass Criteria**: ✅ Appropriate goal classification AND relevant response

---

## Test Suite 3: UI/UX Verification

### Test 3.1: Welcome Screen
**Objective**: Verify initial user experience

**Steps**:
1. Open application in fresh browser tab
2. Observe the welcome screen

**Expected Result**:
- Welcome icon (👋) displays
- "Welcome to SkillTech Navigator!" heading visible
- Welcome message mentions Maruti Makwana
- Four quick action buttons present:
  - Learn Azure
  - AI Fundamentals
  - Career Guidance
  - Certifications

**Pass Criteria**: ✅ All elements visible and properly styled

---

### Test 3.2: Quick Actions
**Objective**: Verify quick action buttons work

**Steps**:
1. Click "Learn Azure" button
2. Verify message appears in chat
3. Verify response is received

**Expected Result**:
- User message appears with "Learn Azure" text
- Welcome screen disappears
- Bot response received
- Goal badge shows "Course Recommendations"

**Pass Criteria**: ✅ Smooth interaction with appropriate response

---

### Test 3.3: Chat Interactions
**Objective**: Verify chat UI elements

**Steps**:
1. Send a message
2. Observe all UI elements

**Expected Result**:
- User message appears on right side
- User avatar (👤) displays
- Bot response appears on left side
- Bot avatar (🤖) displays
- Timestamp shows on both messages
- Smooth slide-in animation

**Pass Criteria**: ✅ All elements properly positioned and animated

---

### Test 3.4: Typing Indicator
**Objective**: Verify typing indicator appears during processing

**Steps**:
1. Type a message
2. Click send button
3. Observe immediately after sending

**Expected Result**:
- Three animated dots appear
- Bot avatar shows with dots
- Dots have pulsing animation
- Indicator disappears when response arrives

**Pass Criteria**: ✅ Typing indicator visible and properly animated

---

### Test 3.5: Responsive Design (Mobile)
**Objective**: Verify mobile responsiveness

**Steps**:
1. Resize browser window to mobile width (375px)
2. OR: Open developer tools (F12) and use device emulation
3. Test all features

**Expected Result**:
- Layout adjusts to mobile size
- Header remains readable
- Chat messages properly sized (max-width 85%)
- Input area remains accessible
- All interactions work smoothly

**Pass Criteria**: ✅ Fully functional on mobile viewport

---

### Test 3.6: Glassmorphism Effect
**Objective**: Verify premium visual design

**Steps**:
1. Open application
2. Observe header and chat container

**Expected Result**:
- Translucent/frosted glass effect visible
- Background gradient animation present
- Subtle blur on containers
- Azure-themed colors (blues, purples)

**Pass Criteria**: ✅ Premium aesthetic maintained

---

## Test Suite 4: Functionality & Error Handling

### Test 4.1: Empty Message Handling
**Objective**: Verify empty messages aren't sent

**Steps**:
1. Leave input field empty
2. Click send button
3. OR: Press Enter with empty field

**Expected Result**:
- No message is sent
- No API call made
- Input field remains focused

**Pass Criteria**: ✅ Empty messages blocked

---

### Test 4.2: Long Messages
**Objective**: Verify handling of lengthy input

**Steps**:
1. Type or paste a very long message (500+ characters)
2. Send the message

**Expected Result**:
- Message sends successfully
- Message displays properly in chat
- Response received
- No UI breaking

**Pass Criteria**: ✅ Long messages handled gracefully

---

### Test 4.3: Conversation History
**Objective**: Verify context is maintained

**Steps**:
1. Send: "My name is John"
2. Wait for response
3. Send: "What's my name?"

**Expected Result**:
- Bot remembers and responds with "John"
- Context from previous messages maintained

**Pass Criteria**: ✅ Bot demonstrates memory of conversation

---

### Test 4.4: API Key Missing
**Objective**: Verify error handling for missing configuration

**Steps**:
1. Remove API key from `appsettings.Development.json`
2. Restart application
3. Try to send a message

**Expected Result**:
- Error message appears in chat
- Message states: "Gemini API key is not configured"
- Application doesn't crash

**Pass Criteria**: ✅ Graceful error message displayed

---

### Test 4.5: Network Error Simulation
**Objective**: Verify handling of connectivity issues

**Steps**:
1. Disconnect internet
2. Try to send a message

**Expected Result**:
- Error message appears
- Message indicates connection problem
- Application remains functional
- Can retry after reconnecting

**Pass Criteria**: ✅ Appropriate error handling

---

## Test Suite 5: Performance & Polish

### Test 5.1: Response Time
**Objective**: Verify reasonable response times

**Steps**:
1. Send message: "Hello"
2. Measure time to response

**Expected Result**:
- Typing indicator appears within 100ms
- Response received within 2-5 seconds (typical)
- UI remains responsive during wait

**Pass Criteria**: ✅ Acceptable performance

---

### Test 5.2: Animation Smoothness
**Objective**: Verify animations are smooth

**Steps**:
1. Send multiple messages in sequence
2. Observe all animations

**Expected Result**:
- Message slide-in: smooth
- Typing dots: smooth pulse
- Button hover: smooth scale
- No jank or stuttering

**Pass Criteria**: ✅ 60 FPS animations

---

### Test 5.3: Browser Compatibility
**Objective**: Verify cross-browser support

**Browsers to Test**:
- [ ] Google Chrome (latest)
- [ ] Microsoft Edge (latest)
- [ ] Firefox (latest)
- [ ] Safari (if on Mac)

**Steps for Each**:
1. Open application
2. Run basic functionality tests
3. Verify visual appearance

**Expected Result**:
- All features work
- Visual design consistent
- No console errors

**Pass Criteria**: ✅ Functional in all tested browsers

---

## Test Suite 6: Content Quality

### Test 6.1: Azure Expertise
**Objective**: Verify agent knowledge of Azure

**Test Questions**:
- "What is Azure?"
- "Tell me about AZ-900"
- "What's the difference between AZ-104 and AZ-305?"

**Expected Result**:
- Accurate information
- Mentions certifications appropriately
- Provides actionable guidance

**Pass Criteria**: ✅ Demonstrates Azure knowledge

---

### Test 6.2: Career Guidance
**Objective**: Verify career mentorship quality

**Test Questions**:
- "I want a career in cloud computing"
- "How do I become an Azure developer?"
- "What skills do I need?"

**Expected Result**:
- Provides career path information
- Suggests learning steps
- Mentions relevant certifications
- Encouraging tone

**Pass Criteria**: ✅ Helpful career guidance provided

---

## 📊 Test Results Template

Use this template to record your test results:

```
Test Date: _______________
Tester: __________________
Application Version: _______________

┌──────────────────────────────────────┬──────┬────────┐
│ Test Case                            │ Pass │ Notes  │
├──────────────────────────────────────┼──────┼────────┤
│ 1.1 Agent Identity                   │ ☐    │        │
│ 1.2 Tone and Personality             │ ☐    │        │
│ 2.1 Course Recommendations Goal      │ ☐    │        │
│ 2.2 Sales Goal                       │ ☐    │        │
│ 2.3 Support Goal                     │ ☐    │        │
│ 2.4 General Conversation             │ ☐    │        │
│ 3.1 Welcome Screen                   │ ☐    │        │
│ 3.2 Quick Actions                    │ ☐    │        │
│ 3.3 Chat Interactions                │ ☐    │        │
│ 3.4 Typing Indicator                 │ ☐    │        │
│ 3.5 Responsive Design                │ ☐    │        │
│ 3.6 Glassmorphism Effect             │ ☐    │        │
│ 4.1 Empty Message Handling           │ ☐    │        │
│ 4.2 Long Messages                    │ ☐    │        │
│ 4.3 Conversation History             │ ☐    │        │
│ 4.4 API Key Missing                  │ ☐    │        │
│ 4.5 Network Error Simulation         │ ☐    │        │
│ 5.1 Response Time                    │ ☐    │        │
│ 5.2 Animation Smoothness             │ ☐    │        │
│ 5.3 Browser Compatibility            │ ☐    │        │
│ 6.1 Azure Expertise                  │ ☐    │        │
│ 6.2 Career Guidance                  │ ☐    │        │
└──────────────────────────────────────┴──────┴────────┘

Overall Pass Rate: _____ / 21 tests

Critical Issues Found:
-
-

Recommendations:
-
-
```

---

## 🎯 Success Criteria

For production readiness, aim for:
- ✅ **100% pass rate** on Tests 1.1-1.2 (Persona)
- ✅ **100% pass rate** on Tests 2.1-2.3 (Core Goals)
- ✅ **100% pass rate** on Tests 4.1-4.5 (Error Handling)
- ✅ **80%+ pass rate** on UI/UX tests
- ✅ **Zero critical bugs**

---

## 🐛 Bug Report Template

When you find issues, document them:

```
Bug #: ______
Severity: [ ] Critical  [ ] High  [ ] Medium  [ ] Low
Test Case: ______________

Description:


Steps to Reproduce:
1.
2.
3.

Expected Result:


Actual Result:


Screenshots/Logs:


Environment:
- Browser: ______________
- OS: ______________
- .NET Version: ______________
```

---

**Happy Testing! 🚀**
