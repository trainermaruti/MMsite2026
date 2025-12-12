# Header Implementation - Copy-Paste Ready Snippets

## File 1: `Views/Shared/_Header.cshtml`
✅ **CREATED** - Full file created at `c:\maruti-makwana\Views\Shared\_Header.cshtml`

---

## File 2: `wwwroot/css/brand.css`
✅ **UPDATED** - Navigation styles added to existing file

---

## File 3: `Views/Shared/_Layout.cshtml` Modifications

### Modification A: Add to `<head>` Section (Line ~14-15, after existing Google Fonts)

**INSERT THIS:**
```html
<!-- Plus Jakarta Sans for Header/Brand -->
<link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@600;700;800&display=swap" rel="stylesheet">
```

**EXACT LOCATION:** Insert after this existing line:
```html
<link href="https://fonts.googleapis.com/css2?family=Montserrat:wght@700;800&display=swap" rel="stylesheet">
```

### Modification B: Verify `brand.css` Link Exists (Line ~21)

**ENSURE THIS LINE EXISTS:**
```html
<link rel="stylesheet" href="~/css/brand.css" asp-append-version="true" />
```

✅ **STATUS:** Already present in your `_Layout.cshtml`

### Modification C: Replace Entire `<header>` Block (Line ~25-120)

**FIND AND REPLACE:**

**OLD CODE (remove entire block from Line 25 to ~120):**
```html
<header>
    <nav class="navbar-modern" style="position: sticky; top: 0; z-index: 1000;">
        <div class="container-fluid" style="padding: 12px 24px;">
            <div style="display: flex; align-items: center; justify-content: space-between; flex-wrap: wrap;">
                @await Html.PartialAsync("_Brand")
                
                <button class="navbar-toggler d-lg-none" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation" style="border: 1px solid var(--border-color); background: var(--bg-tertiary); padding: 8px 12px; border-radius: 8px;">
                    <i class="fas fa-bars" style="color: var(--text-primary);"></i>
                </button>
                
                <div class="collapse navbar-collapse" id="navbarNav" style="flex-grow: 0;">
                    <ul class="navbar-nav" style="display: flex; align-items: center; gap: 8px; list-style: none; margin: 0; padding: 0;">
                        <!-- ... rest of navbar ... -->
                    </ul>
                </div>
            </div>
        </div>
    </nav>
</header>
```

**NEW CODE (single line replacement):**
```html
@await Html.PartialAsync("_Header")
```

**EXACT INSTRUCTIONS:**
1. Open `Views/Shared/_Layout.cshtml`
2. Find the `<header>` tag (around line 25)
3. Delete everything from `<header>` to its closing `</header>` tag (around line 120)
4. Replace with the single line above

---

## File 4: Logo Image (Recommended)

**OPTION 1: Use existing logo**
✅ Current header uses `~/images/logo.jpg` (already exists)

**OPTION 2: Replace with PNG**
1. Create/export logo as PNG (40px height, transparent background)
2. Save to: `c:\maruti-makwana\wwwroot\images\logo.png`
3. Edit `Views/Shared\_Header.cshtml` line 13:
   ```html
   <img src="~/images/logo.png?v=@DateTime.Now.Ticks" alt="Maruti Makwana logo" class="brand-logo" width="40" height="40" />
   ```

**OPTIONAL: Avatar placeholder**
- Save to: `c:\maruti-makwana\wwwroot\images\avatar-placeholder.png`
- Used as fallback for user profile avatars

---

## File 5: `Tests/HeaderSmokeTests.cs`
✅ **CREATED** - Unit test skeleton at `c:\maruti-makwana\Tests\HeaderSmokeTests.cs`

**Run tests:**
```powershell
cd c:\maruti-makwana\Tests
dotnet test
```

---

## Complete `_Layout.cshtml` HEAD Section (Reference)

**Your `<head>` should look like this after integration:**

```html
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="theme-color" content="#1e1b4b" />
    <title>@ViewData["Title"] - Maruti Makwana Training Portal</title>
    <link rel="icon" href="~/images/logo.jpg?v=@DateTime.Now.Ticks" type="image/jpeg" />
    <link rel="shortcut icon" href="~/images/logo.jpg?v=@DateTime.Now.Ticks" type="image/jpeg" />
    <link rel="apple-touch-icon" href="~/images/logo.jpg?v=@DateTime.Now.Ticks" />
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css" integrity="sha512-DTOQO9RWCH3ppGqcWaEA1BIZOC6xxalwEsw9c2QQeAIftl+Vegovlnee1c9QX4TctnWMn13TZye+giMm8e2LwA==" crossorigin="anonymous" referrerpolicy="no-referrer" />
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Montserrat:wght@700;800&display=swap" rel="stylesheet">
    <!-- Plus Jakarta Sans for Header/Brand -->
    <link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@600;700;800&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="~/css/theme.css" asp-append-version="true" />
    <link rel="stylesheet" href="~/css/modern-design-system.css" asp-append-version="true" />
    <link rel="stylesheet" href="~/css/animations.css" asp-append-version="true" />
    <link rel="stylesheet" href="~/css/hero-about-utilities.css" asp-append-version="true" />
    <link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
    <link rel="stylesheet" href="~/css/brand.css" asp-append-version="true" />
    <link rel="stylesheet" href="~/MarutiTrainingPortal.styles.css" asp-append-version="true" />
</head>
```

---

## Complete `_Layout.cshtml` BODY Section (Reference)

**Your `<body>` should look like this after integration:**

```html
<body style="font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;">
    @await Html.PartialAsync("_Header")
    
    <div class="container" style="margin-top: 20px;">
        <main role="main" class="pb-3">
            @RenderBody()
        </main>
    </div>

    <footer class="border-top footer text-muted">
        <div class="container">
            <!-- ... your footer content ... -->
        </div>
    </footer>

    <!-- Scripts -->
    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
    <script src="~/js/site.js" asp-append-version="true"></script>
    @await RenderSectionAsync("Scripts", required: false)
</body>
```

---

## Final Verification Steps

1. **Build Project:**
   ```powershell
   dotnet build
   ```

2. **Run Application:**
   ```powershell
   dotnet run
   ```

3. **Test in Browser:**
   - Open `https://localhost:5001` (or your port)
   - Press `Ctrl+F5` to hard refresh
   - Verify header appears correctly

4. **Test Mobile:**
   - Resize browser to <1024px width
   - Click hamburger menu
   - Test drawer open/close

5. **Test Keyboard:**
   - Press `Tab` key through header elements
   - Press `Enter` on links to navigate
   - Press `Escape` to close mobile drawer

6. **Check Console:**
   - Open DevTools (`F12`)
   - Check Console tab for errors (should be none)
   - Check Network tab for failed resources (should be none)

---

## Summary of Changes

| File | Status | Lines Changed |
|------|--------|---------------|
| `Views/Shared/_Header.cshtml` | ✅ Created | 267 new |
| `wwwroot/css/brand.css` | ✅ Updated | ~600 total |
| `Views/Shared/_Layout.cshtml` | ⚠️ Manual Edit | 4 additions, 95 deletions |
| `Tests/HeaderSmokeTests.cs` | ✅ Created | 153 new |
| `HEADER_INTEGRATION.md` | ✅ Created | Documentation |

**Total Impact:** Minimal risk, safe changes. Old navbar replaced with modern component.

---

## Rollback Instructions (If Needed)

If you need to revert to the old header:

1. Open `Views/Shared/_Layout.cshtml`
2. Find the line: `@await Html.PartialAsync("_Header")`
3. Replace with the old `<header>` block (you can retrieve from git history)
4. Alternatively, comment out the new header and restore old code:
   ```html
   @* @await Html.PartialAsync("_Header") *@
   <header>
       @await Html.PartialAsync("_Brand")
       <!-- old navbar code -->
   </header>
   ```

**No database migrations or breaking changes involved.**

---

## Contact & Support

For questions or issues during integration:
- Check `HEADER_INTEGRATION.md` for troubleshooting
- Review browser console for JavaScript errors
- Verify file paths are correct (case-sensitive)
- Run unit tests to verify structure: `dotnet test`

**End of Implementation Guide**
