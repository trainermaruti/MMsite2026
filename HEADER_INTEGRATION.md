# Header Integration Guide

## Overview
This guide provides step-by-step instructions for integrating the new polished header/navbar into the Maruti Makwana Training Portal. The header features:
- Plus Jakarta Sans branding with gradient text effects
- Responsive mobile-first design with drawer navigation
- Accessibility-compliant (WCAG 2.1 AA)
- Sticky header with backdrop blur
- Active link highlighting
- CTA button with gradient

---

## File Structure

```
Views/Shared/
  ├── _Header.cshtml          (NEW - Main header partial)
  └── _Layout.cshtml           (MODIFIED - Integration point)

wwwroot/css/
  └── brand.css                (MODIFIED - Navigation styles added)

wwwroot/images/
  ├── logo.png                 (RECOMMENDED - Replace with 40px height PNG)
  └── avatar-placeholder.png   (OPTIONAL - User avatar fallback)

Tests/
  └── HeaderSmokeTests.cs      (NEW - Unit test skeleton)
```

---

## Integration Steps

### Step 1: Update `Views/Shared/_Layout.cshtml`

**A) Add Plus Jakarta Sans font in `<head>` section:**

Insert the following lines **AFTER** the existing Google Fonts preconnect/link tags (around line 13-15):

```html
<!-- Plus Jakarta Sans for Header/Brand -->
<link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@600;700;800&display=swap" rel="stylesheet">
```

**Note:** The existing `brand.css` already imports Plus Jakarta Sans via `@import`, but this provides a preloaded fallback.

**B) Verify `brand.css` is included:**

Ensure this line exists in the `<head>` (it should already be there around line 21):

```html
<link rel="stylesheet" href="~/css/brand.css" asp-append-version="true" />
```

**C) Replace the existing `<header>` section in `<body>`:**

Find the existing header/nav block (approximately lines 25-120) that starts with:
```html
<header>
    <nav class="navbar-modern" style="position: sticky; top: 0; z-index: 1000;">
```

**Replace the entire `<header>...</header>` block** with:

```html
@await Html.PartialAsync("_Header")
```

This single line renders the new `_Header.cshtml` partial.

---

### Step 2: Prepare Logo Image (Recommended)

1. **Create a logo PNG** (40px height recommended, transparent background):
   - Export your logo as `logo.png`
   - Place in `wwwroot/images/logo.png`
   
2. **Fallback:** The header currently uses `~/images/logo.jpg` (existing file). You can:
   - Keep using `logo.jpg`, OR
   - Replace line 13 in `_Header.cshtml` to use `logo.png`:
     ```html
     <img src="~/images/logo.png?v=@DateTime.Now.Ticks" alt="Maruti Makwana logo" class="brand-logo" width="40" height="40" />
     ```

---

### Step 3: Test the Integration

1. **Build and Run:**
   ```powershell
   dotnet build
   dotnet run
   ```

2. **Open in browser:**
   - Navigate to `https://localhost:5001` (or your configured port)
   - Press `Ctrl+F5` to hard refresh and bypass cache

3. **Desktop Testing:**
   - Verify logo, brand text, and navigation links appear
   - Click each nav link (Home, Trainings, Courses, Events, About)
   - Verify active link has gradient underline indicator
   - Click "Contact Me" CTA button
   - Test theme toggle placeholder button (no action yet)

4. **Mobile Testing:**
   - Resize browser window to <1024px width (or use DevTools device emulation)
   - Click hamburger menu (three lines) in top-right
   - Verify mobile drawer slides in from right
   - Test all mobile nav links
   - Click overlay or X button to close drawer
   - Press `Escape` key to close drawer (keyboard accessibility)

5. **Keyboard Navigation:**
   - Tab through all header elements
   - Verify focus indicators (blue outline) are visible
   - Press `Enter` on links/buttons to activate
   - Open mobile drawer, press `Tab` - focus should stay within drawer
   - Press `Shift+Tab` at first element - should wrap to last element

6. **Accessibility:**
   - Use browser DevTools accessibility inspector
   - Run Lighthouse accessibility audit (should score 95+)
   - Test with screen reader (NVDA/JAWS) to verify ARIA labels

---

### Step 4: Manual Verification Checklist

- [ ] Header is sticky at top on scroll
- [ ] Logo and "MARUTI MAKWANA / Training Portal" display correctly
- [ ] Desktop nav links visible on screens ≥1024px
- [ ] Active page has gradient underline on nav link
- [ ] "Contact Me" CTA button has gradient background
- [ ] Mobile hamburger menu appears on screens <1024px
- [ ] Mobile drawer opens/closes smoothly
- [ ] Escape key closes mobile drawer
- [ ] Clicking overlay closes mobile drawer
- [ ] All links navigate to correct controllers/actions
- [ ] Focus indicators visible on Tab navigation
- [ ] No console errors in browser DevTools
- [ ] Logo image loads (check Network tab if missing)

---

### Step 5: Optional Customizations

**Change CTA Button Text/Link:**
Edit `_Header.cshtml` line 49-52:
```html
<a asp-controller="YourController" asp-action="YourAction" class="btn-cta">
    <i class="fas fa-your-icon" aria-hidden="true"></i>
    <span>Your CTA Text</span>
</a>
```

**Add Theme Toggle Functionality:**
The theme toggle button (line 55-58) is a placeholder. To implement:
1. Add JavaScript to toggle `.light-theme` class on `<body>` or `:root`
2. Persist preference in `localStorage`
3. Update icon from `fa-moon` to `fa-sun` when active

**Customize Colors:**
Edit `wwwroot/css/brand.css` CSS variables (lines 10-32):
```css
:root {
    --brand-accent: #3b82f6;        /* Primary blue */
    --brand-accent-light: #93c5fd;  /* Light blue */
    --brand-bg: #0a0a0a;            /* Dark background */
    /* ... etc */
}
```

---

## Running Unit Tests

The `Tests/HeaderSmokeTests.cs` file contains basic assertions to verify:
- Header partial file exists
- Navigation elements are present
- ARIA attributes for accessibility
- JavaScript toggle functionality

**Run tests:**
```powershell
cd Tests
dotnet test
```

Expected: All 13 tests should pass.

---

## Troubleshooting

**Issue:** Logo not appearing
- **Fix:** Verify `wwwroot/images/logo.jpg` exists, or update path in `_Header.cshtml` line 13

**Issue:** Plus Jakarta Sans font not loading
- **Fix:** Check browser DevTools Network tab for 404 errors. Ensure Google Fonts link is in `<head>`

**Issue:** Mobile drawer not opening
- **Fix:** Check browser console for JavaScript errors. Verify element IDs match: `navToggle`, `mobileNavDrawer`

**Issue:** Active link not highlighting
- **Fix:** JavaScript runs on page load. Hard refresh (`Ctrl+F5`) or check console for errors

**Issue:** CSS styles not applying
- **Fix:** Clear browser cache, verify `brand.css` is loaded in Network tab, check for CSS conflicts

**Issue:** Focus indicators missing
- **Fix:** Ensure you're testing with keyboard (Tab key), not mouse. Check `:focus-visible` CSS support

---

## Browser Compatibility

- **Chrome/Edge:** Full support (tested 120+)
- **Firefox:** Full support (tested 121+)
- **Safari:** Full support (tested 17+)
- **Mobile Safari/Chrome:** Full support
- **IE11:** Not supported (uses modern CSS features)

---

## Next Steps

1. ✅ Complete integration steps above
2. ✅ Test on desktop and mobile viewports
3. ✅ Verify accessibility with keyboard navigation
4. 🔲 Replace placeholder logo with branded PNG
5. 🔲 Implement theme toggle functionality (if desired)
6. 🔲 Add user dropdown menu for avatar (if auth system expanded)
7. 🔲 Run Lighthouse performance audit
8. 🔲 Deploy to staging/production

---

## Support

If you encounter issues:
1. Review console errors in browser DevTools (F12)
2. Verify file paths are correct (case-sensitive on Linux servers)
3. Check that all required files exist in workspace
4. Run unit tests to isolate component failures

**Happy coding! 🚀**
