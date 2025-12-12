# Admin Messages Inbox - Quick Reference Card

## 🚀 Access
**URL:** `https://yourdomain.com/Admin/Messages`  
**Auth:** Admin role required  
**Menu:** Sidebar → Messages (inbox icon)

## 📊 Dashboard Stats
| Stat | Description | Action |
|------|-------------|--------|
| 🔵 New Messages | Unread count | Click to filter unread only |
| ⚪ Total Messages | All non-deleted | Click to show all |

## 🔍 Search & Filter
```
Search: Name, Email, Subject, Message content
Filter: All | New (unread only)
Clear: X button (when search active)
```

## 📧 Message Actions

### View Message
- **Action:** Click eye icon or row
- **Result:** Opens modal, auto-marks as read
- **Display:** Full details + reply options

### Reply via Email
- **Button:** 📧 Reply via Email
- **Action:** Opens mailto: link
- **Pre-fills:** Subject (Re: Original)

### Reply via WhatsApp
- **Button:** 💬 Reply via WhatsApp
- **Condition:** Phone number exists
- **Action:** Opens WhatsApp Web/App

### Mark as Unread
- **Button:** Mark as Unread
- **Action:** Toggles read status
- **Result:** Blue dot returns

### Delete Message
- **Action:** Click trash icon
- **Confirm:** Yes/No modal
- **Result:** Soft delete (recoverable from DB)

## 📤 Export CSV
- **Button:** Export CSV (top-right)
- **Format:** `contact-messages-YYYYMMDD-HHmmss.csv`
- **Columns:** Id, Name, Email, Phone, Subject, Message, Created, IsRead, Status, EventId
- **Filter:** Respects current filter (All/New)

## 🎨 Visual Indicators

### Unread Message
- ✅ Blue dot (8px circle with glow)
- ✅ Highlighted row background
- ✅ Bold subject text

### Read Message
- ✅ No dot
- ✅ Normal background
- ✅ Regular text weight

## ⌨️ Keyboard Shortcuts
- `Enter` in search: Submit search
- `Esc` in modal: Close modal

## 📱 Mobile Responsive
- Stats cards: Stack vertically
- Table: Reduced padding
- Hide: Secondary email, time
- Subject: Truncate to 150px

## 🔧 Admin Tasks

### Seed Test Data
```powershell
Get-Content Migrations/SeedContactMessages.sql | sqlite3 MarutiTrainingPortal.db
```

### Check Message Counts
```sql
SELECT COUNT(*) FROM ContactMessages WHERE IsDeleted = 0;
SELECT COUNT(*) FROM ContactMessages WHERE IsDeleted = 0 AND IsRead = 0;
```

### Recover Deleted Message
```sql
UPDATE ContactMessages SET IsDeleted = 0 WHERE Id = 123;
```

### Bulk Mark as Read
```sql
UPDATE ContactMessages SET IsRead = 1, UpdatedDate = datetime('now') 
WHERE IsDeleted = 0 AND IsRead = 0;
```

## 🐛 Quick Troubleshooting

| Issue | Solution |
|-------|----------|
| 404 on /Admin/Messages | Check admin role, ensure controller exists |
| Modal not opening | Check Bootstrap 5 JS loaded |
| AJAX errors | Check anti-forgery token |
| No messages | Verify IsDeleted = 0 in database |
| WhatsApp missing | Check PhoneNumber field populated |

## 📚 API Endpoints Reference

```
GET  /Admin/Messages?page=1&q=search&filter=All
GET  /Admin/Messages/GetMessage/123
POST /Admin/Messages/MarkAsRead/123
POST /Admin/Messages/Delete/123
GET  /Admin/Messages/Export?filter=New
```

## 🎯 Common Use Cases

### Morning Routine
1. Navigate to /Admin/Messages
2. Click "New Messages" filter
3. Review unread inquiries
4. Reply via email/WhatsApp
5. Mark important as unread for follow-up

### Weekly Export
1. Click "Export CSV"
2. Save to `reports/messages-YYYY-MM-DD.csv`
3. Import to CRM or Excel for analysis

### Search for Customer
1. Enter customer email in search
2. View conversation history
3. Reply or follow up

### Clean Up Old Messages
1. Filter by "All"
2. Review old read messages
3. Delete spam or resolved inquiries

## 🔗 Related Documentation
- Full guide: `README_MESSAGES.md`
- Implementation: `IMPLEMENTATION_MESSAGES_INBOX.md`
- Model: `Models/ContactMessage.cs`
- Controller: `Areas/Admin/Controllers/MessagesController.cs`

---

**Version:** 1.0  
**Last Updated:** December 2025
