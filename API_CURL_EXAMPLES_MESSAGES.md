# Admin Messages API - cURL Examples

## Prerequisites

1. **Login as Admin** first to get authentication cookie
2. **Get Anti-Forgery Token** from the page HTML:
   ```bash
   curl -c cookies.txt https://yourdomain.com/Admin/Messages
   ```

3. **Extract Token** from HTML (look for `__RequestVerificationToken` hidden input)

## Example Requests

### 1. Get Inbox (First Page, All Messages)

```bash
curl -b cookies.txt \
  "https://yourdomain.com/Admin/Messages?page=1&filter=All"
```

### 2. Get Inbox with Search

```bash
curl -b cookies.txt \
  "https://yourdomain.com/Admin/Messages?page=1&q=john&filter=All"
```

### 3. Filter New (Unread) Messages Only

```bash
curl -b cookies.txt \
  "https://yourdomain.com/Admin/Messages?page=1&filter=New"
```

### 4. Get Message Details (AJAX)

```bash
curl -b cookies.txt \
  -H "Accept: application/json" \
  "https://yourdomain.com/Admin/Messages/GetMessage/123"
```

**Expected Response:**
```json
{
  "success": true,
  "id": 123,
  "name": "John Doe",
  "email": "john@example.com",
  "phoneNumber": "1234567890",
  "subject": "Training Inquiry",
  "message": "I am interested in your corporate training programs...",
  "createdDate": "Dec 01, 2025 at 3:45 PM",
  "isRead": true,
  "status": "New",
  "eventId": null,
  "eventTitle": ""
}
```

### 5. Mark Message as Read

```bash
curl -b cookies.txt \
  -X POST \
  -H "Content-Type: application/json" \
  -H "RequestVerificationToken: YOUR_TOKEN_HERE" \
  -d '{"isRead": true}' \
  "https://yourdomain.com/Admin/Messages/MarkAsRead/123"
```

**Expected Response:**
```json
{
  "success": true,
  "isRead": true
}
```

### 6. Mark Message as Unread

```bash
curl -b cookies.txt \
  -X POST \
  -H "Content-Type: application/json" \
  -H "RequestVerificationToken: YOUR_TOKEN_HERE" \
  -d '{"isRead": false}' \
  "https://yourdomain.com/Admin/Messages/MarkAsRead/123"
```

**Expected Response:**
```json
{
  "success": true,
  "isRead": false
}
```

### 7. Delete Message (Soft Delete)

```bash
curl -b cookies.txt \
  -X POST \
  -H "RequestVerificationToken: YOUR_TOKEN_HERE" \
  "https://yourdomain.com/Admin/Messages/Delete/123"
```

**Expected Response:**
```json
{
  "success": true
}
```

### 8. Export All Messages to CSV

```bash
curl -b cookies.txt \
  -o messages-all.csv \
  "https://yourdomain.com/Admin/Messages/Export?filter=All"
```

### 9. Export Only New (Unread) Messages to CSV

```bash
curl -b cookies.txt \
  -o messages-new.csv \
  "https://yourdomain.com/Admin/Messages/Export?filter=New"
```

## PowerShell Examples

### Get Message Details

```powershell
$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
$cookie = New-Object System.Net.Cookie("YourCookieName", "YourCookieValue", "/", "yourdomain.com")
$session.Cookies.Add($cookie)

Invoke-RestMethod -Uri "https://yourdomain.com/Admin/Messages/GetMessage/123" `
  -WebSession $session `
  -Method Get
```

### Mark as Read

```powershell
$headers = @{
    "Content-Type" = "application/json"
    "RequestVerificationToken" = "YOUR_TOKEN"
}

$body = @{
    isRead = $true
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://yourdomain.com/Admin/Messages/MarkAsRead/123" `
  -WebSession $session `
  -Method Post `
  -Headers $headers `
  -Body $body
```

### Delete Message

```powershell
$headers = @{
    "RequestVerificationToken" = "YOUR_TOKEN"
}

Invoke-RestMethod -Uri "https://yourdomain.com/Admin/Messages/Delete/123" `
  -WebSession $session `
  -Method Post `
  -Headers $headers
```

### Export to CSV

```powershell
Invoke-WebRequest -Uri "https://yourdomain.com/Admin/Messages/Export?filter=All" `
  -WebSession $session `
  -OutFile "messages.csv"
```

## JavaScript/Fetch Examples

### Get Message (from browser console)

```javascript
fetch('/Admin/Messages/GetMessage/123', {
    method: 'GET',
    headers: {
        'Accept': 'application/json'
    },
    credentials: 'include'
})
.then(response => response.json())
.then(data => console.log(data));
```

### Mark as Read

```javascript
const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

fetch('/Admin/Messages/MarkAsRead/123', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'RequestVerificationToken': token
    },
    body: JSON.stringify({ isRead: true }),
    credentials: 'include'
})
.then(response => response.json())
.then(data => console.log(data));
```

### Delete Message

```javascript
const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

fetch('/Admin/Messages/Delete/123', {
    method: 'POST',
    headers: {
        'RequestVerificationToken': token
    },
    credentials: 'include'
})
.then(response => response.json())
.then(data => console.log(data));
```

## Testing Workflow

### Complete Test Sequence

```bash
#!/bin/bash

# 1. Login and save cookies
curl -c cookies.txt -X POST \
  -d "Email=admin@example.com&Password=yourpassword" \
  https://yourdomain.com/Account/Login

# 2. Get inbox page and extract token
curl -b cookies.txt https://yourdomain.com/Admin/Messages > inbox.html
TOKEN=$(grep -oP '__RequestVerificationToken.*?value="\K[^"]+' inbox.html)

# 3. Get message details
curl -b cookies.txt \
  "https://yourdomain.com/Admin/Messages/GetMessage/1"

# 4. Mark as unread
curl -b cookies.txt -X POST \
  -H "Content-Type: application/json" \
  -H "RequestVerificationToken: $TOKEN" \
  -d '{"isRead": false}' \
  "https://yourdomain.com/Admin/Messages/MarkAsRead/1"

# 5. Mark as read
curl -b cookies.txt -X POST \
  -H "Content-Type: application/json" \
  -H "RequestVerificationToken: $TOKEN" \
  -d '{"isRead": true}' \
  "https://yourdomain.com/Admin/Messages/MarkAsRead/1"

# 6. Export CSV
curl -b cookies.txt \
  -o messages.csv \
  "https://yourdomain.com/Admin/Messages/Export?filter=All"

# 7. Delete message
curl -b cookies.txt -X POST \
  -H "RequestVerificationToken: $TOKEN" \
  "https://yourdomain.com/Admin/Messages/Delete/1"

# Cleanup
rm cookies.txt inbox.html
```

## Error Responses

### 404 Not Found
```json
{
  "success": false,
  "error": "Message not found"
}
```

### 401 Unauthorized
```html
<html>
<head><title>401 Authorization Required</title></head>
...
```

### 403 Forbidden (CSRF validation failed)
```html
<html>
<head><title>400 Bad Request</title></head>
...
```

## Rate Limiting

If you encounter rate limiting:

```json
{
  "error": "Too many requests. Please try again later."
}
```

**Solution:** Add delays between requests:
```bash
sleep 1  # Add 1 second delay
```

## Bulk Operations Script

```powershell
# Mark multiple messages as read
$messageIds = 1, 2, 3, 4, 5

foreach ($id in $messageIds) {
    $body = @{ isRead = $true } | ConvertTo-Json
    
    Invoke-RestMethod `
        -Uri "https://yourdomain.com/Admin/Messages/MarkAsRead/$id" `
        -Method Post `
        -Headers @{ "RequestVerificationToken" = $token } `
        -Body $body `
        -ContentType "application/json"
    
    Write-Host "Marked message $id as read"
    Start-Sleep -Seconds 1
}
```

## Notes

1. **Authentication Required:** All endpoints require admin login
2. **CSRF Protection:** POST endpoints require anti-forgery token
3. **Session Management:** Use cookies for maintaining session
4. **Rate Limiting:** Respect rate limits (if configured)
5. **HTTPS:** Always use HTTPS in production

---

**For more information, see:**
- API Controller: `Areas/Admin/Controllers/MessagesController.cs`
- Full Documentation: `README_MESSAGES.md`
