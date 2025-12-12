# Message Retention & Cleanup Policy

**28-Day Automatic Deletion with Manual Delete Option**

---

## Overview

Contact form messages are now managed with an automatic 28-day retention policy. Messages older than 28 days are automatically soft-deleted, while manual deletion options are available for administrators at any time.

---

## How It Works

### Message Lifecycle

```
Message Submitted
    ↓
IsDeleted = false (Default)
    ↓
Visible in Admin Portal
    ↓
After 21 days: Yellow warning (expiring soon)
    ↓
After 28 days: Auto soft-delete at 2:00 AM UTC
    ↓
IsDeleted = true
    ↓
Hidden from Admin Portal (Query Filter)
    ↓
Preserved in Database (for archive purposes)
```

### Automatic 28-Day Cleanup

**When:** Every day at 2:00 AM UTC
**What:** Messages with CreatedDate older than 28 days
**How:** IsDeleted flag set to true (soft-delete)
**Service:** `MessageCleanupBackgroundService`
**Logging:** All deletions logged in application logs

### Manual Deletion

Administrators can manually delete messages at any time:

1. **From Messages List:**
   - Right-click message → Delete
   - Soft-deleted immediately
   
2. **From Cleanup Dashboard:**
   - Admin → Messages → Cleanup
   - Select messages to delete
   - Click "Delete Now" button

---

## Services & Components

### 1. IMessageCleanupService

**Location:** `Services/IMessageCleanupService.cs`

**Methods:**

```csharp
// Auto-delete messages older than X days
Task<int> DeleteExpiredMessagesAsync(int daysOld = 28);

// Manually delete specific message
Task<bool> DeleteMessageAsync(int messageId);

// Get count of messages ready to delete
Task<int> GetExpiredMessageCountAsync(int daysOld = 28);

// Get messages nearing expiration (21+ days)
Task<List<ContactMessage>> GetExpiringMessagesAsync(int daysOld = 21);
```

### 2. MessageCleanupService

**Location:** `Services/MessageCleanupService.cs`

**Features:**
- Automatic 28-day deletion
- Manual message deletion
- Expiration tracking
- Logging of all operations
- Error handling

### 3. MessageCleanupBackgroundService

**Location:** `Services/MessageCleanupBackgroundService.cs`

**Behavior:**
- Runs automatically daily at 2:00 AM UTC
- Calls `DeleteExpiredMessagesAsync()`
- Logs all cleanup activity
- Error handling with logging
- Non-blocking (async/await)

### 4. MessagesController (Admin)

**Location:** `Areas/Admin/Controllers/MessagesController.cs`

**New Endpoints:**

```
GET  /Admin/Messages/Cleanup           - View cleanup dashboard
POST /Admin/Messages/DeleteExpired     - Manually trigger cleanup
POST /Admin/Messages/DeleteById        - Delete specific message
```

### 5. Cleanup Dashboard View

**Location:** `Areas/Admin/Views/Messages/Cleanup.cshtml`

**Features:**
- Expired messages count (28+ days)
- Expiring messages count (21+ days)
- Table of expiring messages with age
- Manual delete buttons
- Cleanup status information

---

## Registration in Program.cs

```csharp
// Add Message Cleanup Service (28-day auto-delete)
builder.Services.AddScoped<IMessageCleanupService, MessageCleanupService>();

// Add Message Cleanup Background Service
builder.Services.AddHostedService<MarutiTrainingPortal.Services.MessageCleanupBackgroundService>();
```

---

## Database Schema

### ContactMessage Model

```csharp
public class ContactMessage
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; } = false;
    public int? EventId { get; set; }
    public string Status { get; set; } = "New";
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; } = false;  // Soft-delete flag
}
```

### Query Filter

```csharp
modelBuilder.Entity<ContactMessage>()
    .HasQueryFilter(cm => !cm.IsDeleted);
```

This filter automatically excludes soft-deleted messages from all queries.

---

## Admin Cleanup Dashboard

### Access

Navigate to: `/Admin/Messages/Cleanup`

**Requires:** Admin role

### Dashboard Shows

1. **Expired Messages (28+ days)**
   - Count of messages ready to delete
   - One-click delete all expired messages
   - Estimated data to free up

2. **Expiring Soon (21+ days)**
   - Count of messages expiring in next 7 days
   - Table with detailed information
   - Days until auto-deletion

3. **Expiring Messages Table**
   - Sender name and email
   - Message subject
   - Creation date
   - Days old (highlighted by age)
   - Individual delete buttons

---

## Logging

All cleanup operations are logged:

```
INFO: Message Cleanup Background Service started
INFO: Next message cleanup scheduled for 2024-12-13 02:00:00 UTC
INFO: Starting automatic message cleanup
INFO: Message cleanup completed. 5 messages deleted
INFO: Message {MessageId} manually deleted
WARN: Message with ID {MessageId} not found for deletion
ERROR: Error during message cleanup: {ErrorMessage}
```

View logs in: `logs/` directory or application output

---

## Configuration

### Change Auto-Cleanup Time

Edit `MessageCleanupBackgroundService.cs`:

```csharp
var nextRun = now.Date.AddDays(1).AddHours(2); // Change 2 to desired hour
```

### Change Retention Period

Call with different days:

```csharp
// Delete messages older than 30 days instead of 28
await cleanupService.DeleteExpiredMessagesAsync(daysOld: 30);
```

### Disable Auto-Cleanup

In `Program.cs`, comment out:

```csharp
// builder.Services.AddHostedService<MessageCleanupBackgroundService>();
```

---

## Example Usage

### In Controllers

```csharp
private readonly IMessageCleanupService _cleanupService;

public MessagesController(IMessageCleanupService cleanupService)
{
    _cleanupService = cleanupService;
}

public async Task<IActionResult> ManualCleanup()
{
    var deletedCount = await _cleanupService.DeleteExpiredMessagesAsync();
    return Ok(new { message = $"Deleted {deletedCount} messages" });
}
```

### Manual Delete

```csharp
var success = await _cleanupService.DeleteMessageAsync(messageId: 5);
if (success)
{
    // Message deleted
}
```

### Check Expired Messages

```csharp
var expiredCount = await _cleanupService.GetExpiredMessageCountAsync();
var expiringMessages = await _cleanupService.GetExpiringMessagesAsync();
```

---

## Safety & Data Preservation

### Soft-Delete (Not Permanent)

- Messages are NOT permanently deleted
- IsDeleted flag set to true
- Data preserved in database
- Can be restored if needed with:

```csharp
var message = _context.ContactMessages
    .IgnoreQueryFilters()
    .FirstOrDefault(m => m.Id == 5 && m.IsDeleted);

message.IsDeleted = false;
await _context.SaveChangesAsync();
```

### Query Filter Protection

- Query filter automatically excludes deleted messages
- No SQL injection vulnerability
- Transparent to application code
- Works across all repository methods

### Admin Safeguards

- Confirmation before bulk delete
- Manual delete requires admin role
- All deletions logged
- Can view deleted messages in database if needed

---

## Monitoring & Maintenance

### Monitor Auto-Cleanup

1. Check application logs for cleanup messages
2. Visit cleanup dashboard regularly
3. Monitor message count trends
4. Archive important messages before they expire

### Backup Messages

Before large deletions:

```csharp
// Export to CSV from Admin → Messages → Export
GET /Admin/Messages/Export?filter=All
```

---

## Troubleshooting

### Cleanup Not Running

**Problem:** Messages older than 28 days still visible

**Solution:**
1. Check if background service is registered in Program.cs
2. Check application logs for errors
3. Verify DateTime.UtcNow is correct
4. Check if cleanup dashboard shows expired count

### Too Many Messages Deleted

**Problem:** More messages deleted than expected

**Solution:**
1. Check `daysOld` parameter (default is 28)
2. Verify CreatedDate values in database
3. Check if timezones are correct (uses UtcNow)

### Permissions Error

**Problem:** "Unauthorized" on cleanup dashboard

**Solution:**
1. Ensure user has Admin role
2. Check if logged in as admin
3. Verify Authorize(Roles = "Admin") decorator

---

## Future Enhancements

1. **Archive Messages:** Move to archive table instead of soft-delete
2. **Retention Policies:** Different retention for different statuses
3. **Bulk Operations:** Archive multiple messages at once
4. **Audit Trail:** Track who deleted messages and when
5. **Email Digest:** Daily summary of expiring messages
6. **Scheduled Reports:** Weekly/monthly cleanup reports

---

## Summary

✅ **Automatic 28-day retention with daily cleanup at 2:00 AM UTC**
✅ **Manual deletion available to admins anytime**
✅ **Data preserved via soft-delete (not permanently removed)**
✅ **Dashboard for monitoring and managing messages**
✅ **Comprehensive logging of all operations**
✅ **Safe implementation with query filters**

---

**Deployment Ready:** All components tested and documented
**Last Updated:** 2024
**Status:** Production Ready ✅
