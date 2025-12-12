# Admin Messages Inbox - Implementation Guide

## Overview

The Admin Messages Inbox feature provides a complete email-style inbox for managing contact messages submitted through the public contact form. This feature includes:

- ✅ Email-style inbox with unread indicators
- ✅ Search and filter capabilities
- ✅ View messages in a modal with automatic read marking
- ✅ Reply via Email or WhatsApp
- ✅ Soft delete messages
- ✅ Export to CSV
- ✅ Dark theme enterprise UI
- ✅ Fully responsive design

## Files Created

### Backend

1. **Repositories/IContactMessageRepository.cs** - Repository interface
2. **Repositories/ContactMessageRepository.cs** - Repository implementation
3. **Services/IContactMessageService.cs** - Service interface
4. **Services/ContactMessageService.cs** - Service implementation with CSV export
5. **Areas/Admin/Controllers/MessagesController.cs** - Admin controller

### Frontend

6. **Areas/Admin/Views/Messages/Index.cshtml** - Inbox view with modal
7. **wwwroot/css/admin-inbox.css** - Dark theme styles

### Testing

8. **Tests/ContactMessageTests.cs** - Comprehensive xUnit tests

## Setup Instructions

### Step 1: Dependency Injection (Already Done)

The repository and service are registered in `Program.cs`:

```csharp
// Add Contact Message Repository and Service
builder.Services.AddScoped<MarutiTrainingPortal.Repositories.IContactMessageRepository, MarutiTrainingPortal.Repositories.ContactMessageRepository>();
builder.Services.AddScoped<MarutiTrainingPortal.Services.IContactMessageService, MarutiTrainingPortal.Services.ContactMessageService>();
```

### Step 2: Add Navigation to Admin Sidebar

Edit `Areas/Admin/Views/Shared/_ModernAdminLayout.cshtml` and add this menu item:

```html
<!-- Add this after the Trainings menu item, around line 185 -->
<a href="/Admin/Messages" class="modern-sidebar-item @(ViewContext.RouteData.Values["controller"]?.ToString() == "Messages" ? "active" : "")">
    <div class="modern-sidebar-icon"><i class="fas fa-inbox"></i></div>
    <span>Messages</span>
</a>
```

### Step 3: Database Migration (Optional)

The `ContactMessage` model already has all required fields:
- `IsRead` (boolean)
- `IsDeleted` (boolean)
- `CreatedDate` (DateTime)

If you need to add these fields to an existing database, create a migration:

```powershell
dotnet ef migrations add AddMessagesInboxFields
dotnet ef database update
```

### Step 4: Seed Test Data (Optional)

To test the inbox with sample data, you can use the admin seeder or manually insert:

```sql
INSERT INTO ContactMessages (Name, Email, PhoneNumber, Subject, Message, IsRead, IsDeleted, Status, CreatedDate)
VALUES 
('John Doe', 'john@example.com', '1234567890', 'Training Inquiry', 'I am interested in your corporate training programs.', 0, 0, 'New', datetime('now')),
('Jane Smith', 'jane@example.com', '0987654321', 'Certification Question', 'Do you provide certificates?', 0, 0, 'New', datetime('now', '-1 day')),
('Bob Johnson', 'bob@example.com', NULL, 'Corporate Training', 'We need training for 50 employees.', 1, 0, 'Contacted', datetime('now', '-2 days'));
```

## Features & Usage

### 1. Inbox View (`/Admin/Messages`)

**Stats Bar:**
- **New Messages** - Shows count of unread messages (clickable filter)
- **Total Messages** - Shows total non-deleted messages (clickable filter)

**Search:**
- Server-side search by name, email, subject, or message content
- Submit form or click Search button

**Message List:**
- Blue dot indicator for unread messages
- Highlighted row for unread messages
- Shows sender name, email, subject, received date/time
- Actions: View (eye icon), Delete (trash icon)

### 2. View Message Modal

**Triggered by:** Clicking the eye icon or clicking on a message row

**Features:**
- Displays full message content
- Shows sender details (name, email, phone)
- Shows received date/time
- Displays related event if applicable
- **Automatically marks message as read** when opened

**Actions:**
- **Reply via Email** - Opens `mailto:` link with pre-filled subject
- **Reply via WhatsApp** - Opens WhatsApp Web/App (if phone provided)
- **Mark as Unread** - Toggle read status
- **Close** - Dismiss modal

### 3. Delete Message

**How it works:**
- Soft delete (sets `IsDeleted = true`)
- Confirmation modal before deletion
- Removed from inbox view
- Can be recovered from database if needed

### 4. Export to CSV

**Button:** Top-right "Export CSV" button

**Downloads:** `contact-messages-YYYYMMDD-HHmmss.csv`

**CSV Columns:**
```
Id, Name, Email, PhoneNumber, Subject, Message, CreatedAt, IsRead, Status, EventId
```

**Respects current filter:**
- If viewing "New Messages", exports only unread
- If viewing "All", exports all non-deleted

### 5. Pagination

- 20 messages per page
- Page numbers with ellipsis for large datasets
- Previous/Next navigation
- Current page indicator

## API Endpoints

### GET `/Admin/Messages/Index`

**Query Parameters:**
- `page` (int, default: 1) - Page number
- `q` (string, optional) - Search query
- `filter` (string, default: "All") - Filter: "All" or "New"

**Returns:** HTML view with paginated messages

### GET `/Admin/Messages/GetMessage/{id}`

**Returns:** JSON with message details
**Side Effect:** Marks message as read

```json
{
  "success": true,
  "id": 123,
  "name": "John Doe",
  "email": "john@example.com",
  "phoneNumber": "1234567890",
  "subject": "Training Inquiry",
  "message": "I am interested...",
  "createdDate": "Dec 01, 2025 at 3:45 PM",
  "isRead": true,
  "status": "New",
  "eventId": null,
  "eventTitle": ""
}
```

### POST `/Admin/Messages/MarkAsRead/{id}`

**Body:** `{ isRead: true/false }`
**Headers:** `RequestVerificationToken` (anti-forgery)
**Returns:** JSON `{ success: true, isRead: true/false }`

### POST `/Admin/Messages/Delete/{id}`

**Headers:** `RequestVerificationToken` (anti-forgery)
**Returns:** JSON `{ success: true }`

### GET `/Admin/Messages/Export?filter=All`

**Query Parameters:**
- `filter` (string, optional) - "All" or "New"

**Returns:** CSV file download

## Example cURL Commands

### Mark as Read
```bash
curl -X POST https://yourdomain.com/Admin/Messages/MarkAsRead/123 \
  -H "Content-Type: application/json" \
  -H "Cookie: .AspNetCore.Identity.Application=..." \
  -H "RequestVerificationToken: <token>" \
  -d '{"isRead": true}'
```

### Delete Message
```bash
curl -X POST https://yourdomain.com/Admin/Messages/Delete/123 \
  -H "Cookie: .AspNetCore.Identity.Application=..." \
  -H "RequestVerificationToken: <token>"
```

### Export CSV
```bash
curl -X GET "https://yourdomain.com/Admin/Messages/Export?filter=New" \
  -H "Cookie: .AspNetCore.Identity.Application=..." \
  -o messages.csv
```

## Testing

### Run Unit Tests

```powershell
cd Tests
dotnet test --filter ContactMessageTests
```

### Test Coverage

The test suite includes:
- ✅ Pagination (multiple pages)
- ✅ Filtering (All, New)
- ✅ Search (by name, email, subject, message)
- ✅ Mark as read/unread
- ✅ Soft delete
- ✅ Get message by ID
- ✅ Count new messages
- ✅ Count total messages
- ✅ Export to CSV
- ✅ CSV special character escaping
- ✅ Service layer integration

### Manual Testing Checklist

- [ ] Navigate to `/Admin/Messages`
- [ ] Verify stats show correct counts
- [ ] Click "New Messages" filter
- [ ] Search for a message by name
- [ ] Click View on a message
- [ ] Verify modal opens with correct data
- [ ] Verify message marked as read (blue dot removed)
- [ ] Click "Reply via Email" (opens email client)
- [ ] Click "Reply via WhatsApp" (opens WhatsApp if phone exists)
- [ ] Click "Mark as Unread"
- [ ] Verify message becomes unread (blue dot returns)
- [ ] Close modal
- [ ] Click Delete on a message
- [ ] Confirm deletion
- [ ] Verify message removed from list
- [ ] Click "Export CSV"
- [ ] Open CSV and verify data
- [ ] Test pagination with 25+ messages

## UI/UX Design Details

### Color Palette (Dark Theme)

```css
--bg-primary: #0a0a0a       /* Main background */
--bg-secondary: #161616     /* Cards, inputs */
--text-primary: #ffffff     /* Headings, important text */
--text-secondary: #e5e5e5   /* Body text */
--text-muted: #737373       /* Helper text, timestamps */
--border-color: rgba(156, 163, 175, 0.2)  /* Borders */
--primary: #3b82f6          /* Blue accent */
--unread-dot: #60a5fa       /* Unread indicator */
--row-hover: rgba(59, 130, 246, 0.06)  /* Hover state */
```

### Visual Indicators

**Unread Message:**
- Blue dot (8px circle with soft glow)
- Slightly lighter background: `rgba(96, 165, 250, 0.04)`
- Bold subject text

**Read Message:**
- No dot
- Normal background
- Regular subject text

**Row Hover:**
- Background: `rgba(59, 130, 246, 0.06)`
- Smooth transition

### Responsive Breakpoints

**Mobile (< 768px):**
- Stats cards stack vertically
- Hide sender email
- Hide time (show only date)
- Reduce padding
- Truncate subject to 150px

## Configuration

### Email Reply Template

Edit in `Index.cshtml` around line 470:

```javascript
const subject = encodeURIComponent(`Re: ${currentMessageSubject}`);
const body = encodeURIComponent(`\n\n--- Original message from ${currentMessageName} ---\n`);
```

### WhatsApp Reply Template

Edit in `Index.cshtml` around line 476:

```javascript
const message = encodeURIComponent(`Hello ${currentMessageName}, regarding your inquiry...`);
```

### Messages Per Page

Edit in `MessagesController.cs` line 18:

```csharp
const int pageSize = 20; // Change to desired page size
```

## Security Considerations

✅ **Authorization:** All endpoints protected by `[Authorize(Roles = "Admin")]`  
✅ **Anti-forgery:** POST endpoints require `[ValidateAntiForgeryToken]`  
✅ **HTML Encoding:** Message content is HTML-encoded to prevent XSS  
✅ **Soft Delete:** Messages are never hard-deleted from database  
✅ **Input Validation:** Model validation on all inputs  
✅ **SQL Injection:** Protected by EF Core parameterized queries

## Troubleshooting

### Issue: 404 when navigating to /Admin/Messages

**Solution:** Ensure you added the controller to the Admin area and the route is correct.

### Issue: Messages not showing

**Solution:** Check that `IsDeleted = false` in your database. Run:
```sql
SELECT COUNT(*) FROM ContactMessages WHERE IsDeleted = 0;
```

### Issue: Modal not opening

**Solution:** Check browser console for JavaScript errors. Ensure Bootstrap 5 is loaded.

### Issue: AJAX requests failing

**Solution:** Check anti-forgery token. Add this to your layout if missing:
```html
@Html.AntiForgeryToken()
```

### Issue: Export CSV downloads empty file

**Solution:** Verify repository is returning data. Check filter parameter.

### Issue: WhatsApp button not showing

**Solution:** Ensure message has a `PhoneNumber` value in the database.

## Performance Optimization

- **Pagination:** Only loads 20 messages at a time
- **Eager Loading:** Uses `.Include(m => m.Event)` to avoid N+1 queries
- **Indexing:** Add database index on `CreatedDate` for faster sorting:
  ```sql
  CREATE INDEX IX_ContactMessages_CreatedDate ON ContactMessages(CreatedDate DESC);
  CREATE INDEX IX_ContactMessages_IsDeleted_IsRead ON ContactMessages(IsDeleted, IsRead);
  ```

## Future Enhancements

Potential features to add:
- [ ] Bulk actions (select multiple, mark all as read, bulk delete)
- [ ] Advanced filters (by date range, status, event)
- [ ] Message tagging/labeling
- [ ] Assign messages to team members
- [ ] Internal notes on messages
- [ ] Email templates for quick replies
- [ ] Auto-respond to common inquiries
- [ ] Integration with CRM systems
- [ ] Message priority/importance flags
- [ ] Attachment support

## Support

For issues or questions:
1. Check the troubleshooting section above
2. Review the unit tests for usage examples
3. Check browser console for JavaScript errors
4. Review server logs in `logs/` directory

---

**Version:** 1.0  
**Last Updated:** December 2025  
**Author:** Maruti Training Portal Team
