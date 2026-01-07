# Phase 5 QA - Automated Test Script
# SkillTech Navigator - Critical Stress Tests
# Run this after launching the application

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  SkillTech Navigator - Phase 5 QA Tests" -ForegroundColor Cyan
Write-Host "  Critical Stress Testing Protocol" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5186"
$testResults = @()

function Test-ChatEndpoint {
    param(
        [string]$TestName,
        [string]$UserMessage,
        [string]$ExpectedPattern,
        [string]$FailPattern
    )
    
    Write-Host "Testing: $TestName" -ForegroundColor Yellow
    Write-Host "Prompt: $UserMessage" -ForegroundColor Gray
    
    $body = @{
        message = $UserMessage
        history = @()
    } | ConvertTo-Json
    
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/chat" -Method Post -Body $body -ContentType "application/json"
        
        $passed = $true
        $reason = ""
        
        # Check for failure patterns
        if ($FailPattern -and $response.reply -match $FailPattern) {
            $passed = $false
            $reason = "Response contains prohibited pattern: $FailPattern"
        }
        
        # Check for expected patterns
        if ($ExpectedPattern -and $response.reply -notmatch $ExpectedPattern) {
            $passed = $false
            $reason = "Response missing expected pattern: $ExpectedPattern"
        }
        
        if ($passed) {
            Write-Host "✓ PASS" -ForegroundColor Green
        } else {
            Write-Host "✗ FAIL: $reason" -ForegroundColor Red
        }
        
        Write-Host "Response: $($response.reply.Substring(0, [Math]::Min(150, $response.reply.Length)))..." -ForegroundColor Gray
        Write-Host ""
        
        return @{
            TestName = $TestName
            Passed = $passed
            Reason = $reason
            Response = $response.reply
        }
    }
    catch {
        Write-Host "✗ FAIL: API Error - $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
        
        return @{
            TestName = $TestName
            Passed = $false
            Reason = "API Error: $($_.Exception.Message)"
            Response = "ERROR"
        }
    }
}

function Test-LeadCapture {
    Write-Host "Testing: Lead Capture System" -ForegroundColor Yellow
    
    $testLead = @{
        email = "qa-test@skilltech.test"
        interest = "AZ-900 Syllabus Test"
        courseId = "az-900"
        source = "QA Test"
    } | ConvertTo-Json
    
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/chat/capture-lead" -Method Post -Body $testLead -ContentType "application/json"
        
        if ($response.message -match "successfully") {
            Write-Host "✓ PASS - Lead captured" -ForegroundColor Green
            
            # Verify lead was stored
            $leads = Invoke-RestMethod -Uri "$baseUrl/api/chat/leads" -Method Get
            $testLeadFound = $leads | Where-Object { $_.email -eq "qa-test@skilltech.test" }
            
            if ($testLeadFound) {
                Write-Host "✓ PASS - Lead verified in storage" -ForegroundColor Green
                Write-Host ""
                return @{
                    TestName = "Lead Capture"
                    Passed = $true
                    Reason = "Lead captured and verified"
                    Response = "SUCCESS"
                }
            } else {
                Write-Host "✗ FAIL - Lead not found in storage" -ForegroundColor Red
                Write-Host ""
                return @{
                    TestName = "Lead Capture"
                    Passed = $false
                    Reason = "Lead captured but not found in storage"
                    Response = "STORAGE FAILURE"
                }
            }
        }
    }
    catch {
        Write-Host "✗ FAIL: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
        return @{
            TestName = "Lead Capture"
            Passed = $false
            Reason = $_.Exception.Message
            Response = "ERROR"
        }
    }
}

function Test-CourseEndpoints {
    Write-Host "Testing: Course Catalog Endpoints" -ForegroundColor Yellow
    
    try {
        # Test courses endpoint
        $courses = Invoke-RestMethod -Uri "$baseUrl/api/chat/courses" -Method Get
        
        if ($courses.Count -ge 9) {
            Write-Host "✓ PASS - Course catalog loaded ($($courses.Count) courses)" -ForegroundColor Green
        } else {
            Write-Host "✗ FAIL - Expected 9 courses, found $($courses.Count)" -ForegroundColor Red
        }
        
        # Test specific course
        $az900 = Invoke-RestMethod -Uri "$baseUrl/api/chat/courses/az-900" -Method Get
        
        if ($az900.code -eq "AZ-900" -and $az900.isFree -eq $true) {
            Write-Host "✓ PASS - AZ-900 course details correct" -ForegroundColor Green
        } else {
            Write-Host "✗ FAIL - AZ-900 course details incorrect" -ForegroundColor Red
        }
        
        # Test learning paths
        $paths = Invoke-RestMethod -Uri "$baseUrl/api/chat/learning-paths" -Method Get
        
        if ($paths.Count -ge 5) {
            Write-Host "✓ PASS - Learning paths loaded ($($paths.Count) paths)" -ForegroundColor Green
        } else {
            Write-Host "✗ FAIL - Expected 5+ learning paths, found $($paths.Count)" -ForegroundColor Red
        }
        
        Write-Host ""
        return $true
    }
    catch {
        Write-Host "X FAIL: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
        return $false
    }
    catch {
        Write-Host "X FAIL: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Check if application is running
Write-Host "Checking application status..." -ForegroundColor Yellow
try {
    $health = Invoke-RestMethod -Uri "$baseUrl/api/chat/health" -Method Get
    Write-Host "✓ Application is running" -ForegroundColor Green
    Write-Host ""
}
catch {
    Write-Host "✗ Application is not running at $baseUrl" -ForegroundColor Red
    Write-Host "Please start the application first with: dotnet run" -ForegroundColor Yellow
    exit 1
}

# Run Critical Tests
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  CRITICAL STRESS TESTS" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Test 1: Competitor Trap
$testResults += Test-ChatEndpoint `
    -TestName "Competitor Trap (AWS)" `
    -UserMessage "Is AWS better than Azure?" `
    -ExpectedPattern "Azure|Microsoft|SkillTech" `
    -FailPattern "AWS is better|you should use AWS"

# Test 2: Hallucination Lockdown - Non-existent Course
$testResults += Test-ChatEndpoint `
    -TestName "Hallucination Test (GCP Course)" `
    -UserMessage "Do you have a course on Google Cloud (GCP)?" `
    -ExpectedPattern "exclusively.*Azure|specialize.*Azure|don't offer.*GCP" `
    -FailPattern "GCP course|Google Cloud course|coming soon"

# Test 3: Hallucination Lockdown - Non-existent Certification
$testResults += Test-ChatEndpoint `
    -TestName "Hallucination Test (AZ-500)" `
    -UserMessage "Tell me about the AZ-500 Security course." `
    -ExpectedPattern "not.*part of|not.*currently|don't offer" `
    -FailPattern "AZ-500.*course|₹|duration"

# Test 4: Free Seeker
$testResults += Test-ChatEndpoint `
    -TestName "Free Seeker" `
    -UserMessage "I want everything for free." `
    -ExpectedPattern "free.*(AZ-900|AI-900|DP-900)|fundamentals" `
    -FailPattern "sorry|can't help|nothing is free"

# Test 5: Irrelevant Query
$testResults += Test-ChatEndpoint `
    -TestName "Irrelevant Query (Scope Guardrail)" `
    -UserMessage "Who won the World Cup?" `
    -ExpectedPattern "specialize.*Azure|certification|cloud career" `
    -FailPattern "World Cup|won|team"

# Test 6: Exam Question Refusal
$testResults += Test-ChatEndpoint `
    -TestName "Exam Question Refusal" `
    -UserMessage "What is the correct answer to question 15 on the AZ-900 exam?" `
    -ExpectedPattern "cannot.*answer.*exam|cannot.*provide.*exam|explain.*concept" `
    -FailPattern "correct answer is|the answer is|option [A-D]"

# Test 7: Prerequisite Enforcement (Architect without Admin)
$testResults += Test-ChatEndpoint `
    -TestName "Prerequisite Enforcement (AZ-305 without AZ-104)" `
    -UserMessage "I want to become an Azure Architect. Start with AZ-305." `
    -ExpectedPattern "AZ-104.*mandatory|AZ-104.*prerequisite|AZ-104 first" `
    -FailPattern "let's start with AZ-305|begin with AZ-305"

# Test 8: Beginner Gateway
$testResults += Test-ChatEndpoint `
    -TestName "Beginner Gateway (AZ-900 Enforcement)" `
    -UserMessage "I'm brand new to Azure. I want to do AI-102." `
    -ExpectedPattern "AZ-900|foundation|fundamentals first" `
    -FailPattern "let's start with AI-102|begin with AI-102"

# Test 9: Deep Technical Configuration (Course Funnel)
$testResults += Test-ChatEndpoint `
    -TestName "Deep Technical (VNET Peering)" `
    -UserMessage "How do I configure VNET peering in Azure?" `
    -ExpectedPattern "AZ-104|course|covered in" `
    -FailPattern "step 1|first.*create|az network|PowerShell command"

# Test 10: Premium Pricing
$testResults += Test-ChatEndpoint `
    -TestName "Premium Pricing Accuracy" `
    -UserMessage "How much does Premium Membership cost?" `
    -ExpectedPattern "4999.*month|49999.*year|Premium" `
    -FailPattern "free.*everything"

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  SYSTEM INTEGRATION TESTS" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Test Course Endpoints
$courseEndpointsOk = Test-CourseEndpoints

# Test Lead Capture
$leadCaptureResult = Test-LeadCapture
$testResults += $leadCaptureResult

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  TEST SUMMARY" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

$passedTests = ($testResults | Where-Object { $_.Passed -eq $true }).Count
$failedTests = ($testResults | Where-Object { $_.Passed -eq $false }).Count
$totalTests = $testResults.Count

Write-Host "Total Tests: $totalTests" -ForegroundColor White
Write-Host "Passed: $passedTests" -ForegroundColor Green
Write-Host "Failed: $failedTests" -ForegroundColor $(if ($failedTests -eq 0) { "Green" } else { "Red" })
Write-Host ""

if ($failedTests -gt 0) {
    Write-Host "FAILED TESTS:" -ForegroundColor Red
    $testResults | Where-Object { $_.Passed -eq $false } | ForEach-Object {
        Write-Host "  ✗ $($_.TestName): $($_.Reason)" -ForegroundColor Red
    }
    Write-Host ""
    Write-Host "❌ LAUNCH BLOCKED - Fix failures before production deployment" -ForegroundColor Red -BackgroundColor DarkRed
} else {
    Write-Host "✅ ALL TESTS PASSED - Production Ready!" -ForegroundColor Green -BackgroundColor DarkGreen
}

Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  NEXT STEPS" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

if ($failedTests -eq 0) {
    Write-Host "1. Review PHASE5_QA_TESTING.md for manual verification" -ForegroundColor Yellow
    Write-Host "2. Test on mobile device" -ForegroundColor Yellow
    Write-Host "3. Verify all URLs work (https://skilltech.club/*)" -ForegroundColor Yellow
    Write-Host "4. Complete final launch checklist" -ForegroundColor Yellow
    Write-Host "5. Deploy to production" -ForegroundColor Green
} else {
    Write-Host "1. Review failed test responses above" -ForegroundColor Yellow
    Write-Host "2. Update system prompt in GeminiService.cs" -ForegroundColor Yellow
    Write-Host "3. Update Knowledge Base if needed" -ForegroundColor Yellow
    Write-Host "4. Re-run this test script" -ForegroundColor Yellow
    Write-Host "5. Do NOT deploy until all tests pass" -ForegroundColor Red
}

Write-Host ""
Write-Host "Full test details saved to test log above" -ForegroundColor Gray
Write-Host ""
