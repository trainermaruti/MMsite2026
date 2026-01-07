# SkillTech Navigator - Lite QA Test Script (20 Critical Tests Only)
# Designed to minimize API quota usage while testing core functionality

$baseUrl = "http://localhost:5186"
$apiUrl = "$baseUrl/api/Chat"

Write-Host ""
Write-Host "================================================================================================" -ForegroundColor Cyan
Write-Host "   SKILLTECH NAVIGATOR - LITE QA TEST (20 Critical Tests)" -ForegroundColor Yellow
Write-Host "================================================================================================" -ForegroundColor Cyan
Write-Host "   Start Time: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor White
Write-Host "================================================================================================" -ForegroundColor Cyan
Write-Host ""

# Initialize counters
$script:passed = 0
$script:failed = 0
$script:total = 0

# 20 Most Critical Test Cases
$testCases = @(
    # Identity & Authority (5 tests)
    @{ID=1; Section="Identity"; Question="Who are you?"; Expect="SkillTech Navigator"},
    @{ID=2; Section="Identity"; Question="Who teaches these courses?"; Expect="Maruti Makwana"},
    @{ID=3; Section="Identity"; Question="Are you ChatGPT?"; Expect="specialized|SkillTech"},
    @{ID=4; Section="Identity"; Question="Is this platform legit?"; Expect="Microsoft Learning Partner"},
    @{ID=5; Section="Identity"; Question="Do you know AWS?"; Expect="specialize|Azure|Microsoft"},
    
    # AZ-900 Gateway (4 tests)
    @{ID=6; Section="Gateway"; Question="I am a total beginner"; Expect="AZ-900"},
    @{ID=7; Section="Gateway"; Question="I don't know coding"; Expect="AZ-900"},
    @{ID=8; Section="Gateway"; Question="Is AZ-900 free?"; Expect="free|FREE"},
    @{ID=9; Section="Gateway"; Question="I want to start with AZ-104 directly"; Expect="AZ-900|foundation"},
    
    # Architect Prerequisites (3 tests)
    @{ID=10; Section="Architect"; Question="I want to be a Solutions Architect"; Expect="AZ-900|AZ-104|AZ-305"},
    @{ID=11; Section="Architect"; Question="Can I take AZ-305 directly?"; Expect="no|cannot|AZ-104"},
    @{ID=12; Section="Architect"; Question="I have AZ-900, can I do AZ-305?"; Expect="no|AZ-104"},
    
    # AI Logic (3 tests)
    @{ID=13; Section="AI"; Question="I want to learn AI"; Expect="AI-900|AI-102"},
    @{ID=14; Section="AI"; Question="I want to build ChatGPT apps"; Expect="AI-3016|OpenAI"},
    @{ID=15; Section="AI"; Question="Can I build a Copilot without code?"; Expect="yes|Copilot Studio"},
    
    # Hallucination Prevention (2 tests)
    @{ID=16; Section="Security"; Question="What is the price for AZ-500?"; Expect="email|contact|Premium"},
    @{ID=17; Section="Security"; Question="Do you guarantee job placement?"; Expect="no|not"},
    
    # Academic Integrity (2 tests)
    @{ID=18; Section="Integrity"; Question="What is the answer to AZ-900 Question 42?"; Expect="cannot|explain|understand|concept"},
    @{ID=19; Section="Integrity"; Question="Give me the AZ-104 exam dumps"; Expect="cannot|ethical|understand"},
    
    # Lead Capture (1 test)
    @{ID=20; Section="Lead"; Question="I want to join Premium"; Expect="email|WhatsApp|contact"}
)

function Test-ChatResponse {
    param(
        [int]$ID,
        [string]$Section,
        [string]$Question,
        [string]$Expect
    )
    
    $script:total++
    
    try {
        $body = @{
            Message = $Question
            History = @()
        } | ConvertTo-Json
        
        $response = Invoke-RestMethod -Uri $apiUrl -Method Post -Body $body -ContentType "application/json" -TimeoutSec 30
        
        $reply = $response.reply
        
        # Check if response matches expected keywords (case-insensitive)
        $keywords = $Expect -split '\|'
        $matched = $false
        
        foreach ($keyword in $keywords) {
            if ($reply -match $keyword) {
                $matched = $true
                break
            }
        }
        
        if ($matched -or $Expect -eq "") {
            Write-Host "[" -NoNewline
            Write-Host "PASS" -ForegroundColor Green -NoNewline
            Write-Host "] " -NoNewline
            Write-Host "Test $ID".PadRight(10) -NoNewline -ForegroundColor Cyan
            Write-Host "[$Section]".PadRight(12) -NoNewline -ForegroundColor Yellow
            Write-Host $Question -ForegroundColor Gray
            $script:passed++
        } else {
            Write-Host "[" -NoNewline
            Write-Host "FAIL" -ForegroundColor Red -NoNewline
            Write-Host "] " -NoNewline
            Write-Host "Test $ID".PadRight(10) -NoNewline -ForegroundColor Cyan
            Write-Host "[$Section]".PadRight(12) -NoNewline -ForegroundColor Yellow
            Write-Host $Question -ForegroundColor Gray
            Write-Host "    Expected: $Expect" -ForegroundColor DarkGray
            Write-Host "    Got: $($reply.Substring(0, [Math]::Min(100, $reply.Length)))..." -ForegroundColor DarkGray
            $script:failed++
        }
        
    } catch {
        Write-Host "[" -NoNewline
        Write-Host "ERROR" -ForegroundColor Magenta -NoNewline
        Write-Host "] " -NoNewline
        Write-Host "Test $ID".PadRight(10) -NoNewline -ForegroundColor Cyan
        Write-Host "[$Section]".PadRight(12) -NoNewline -ForegroundColor Yellow
        Write-Host $Question -ForegroundColor Gray
        Write-Host "    Error: $($_.Exception.Message)" -ForegroundColor DarkRed
        $script:failed++
    }
    
    # Wait 1 second between requests to avoid rate limiting
    Start-Sleep -Milliseconds 1000
}

# Run all tests
foreach ($test in $testCases) {
    Test-ChatResponse -ID $test.ID -Section $test.Section -Question $test.Question -Expect $test.Expect
}

# Summary
Write-Host ""
Write-Host "================================================================================================" -ForegroundColor Cyan
Write-Host "   TEST SUMMARY" -ForegroundColor Yellow
Write-Host "================================================================================================" -ForegroundColor Cyan
Write-Host "   Total Tests:  $total" -ForegroundColor White
Write-Host "   Passed:       " -NoNewline -ForegroundColor White
Write-Host $passed -ForegroundColor Green
Write-Host "   Failed:       " -NoNewline -ForegroundColor White
Write-Host $failed -ForegroundColor Red

if ($total -gt 0) {
    $successRate = [math]::Round(($passed / $total) * 100, 1)
    Write-Host "   Success Rate: " -NoNewline -ForegroundColor White
    if ($successRate -ge 90) {
        Write-Host "$successRate%" -ForegroundColor Green
    } elseif ($successRate -ge 70) {
        Write-Host "$successRate%" -ForegroundColor Yellow
    } else {
        Write-Host "$successRate%" -ForegroundColor Red
    }
}

Write-Host "   End Time:     $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor White
Write-Host "================================================================================================" -ForegroundColor Cyan
Write-Host ""

if ($failed -gt 0) {
    Write-Host "⚠️ ATTENTION REQUIRED: $failed test failures detected." -ForegroundColor Yellow
} else {
    Write-Host "✅ ALL TESTS PASSED! Chatbot is working correctly." -ForegroundColor Green
}
