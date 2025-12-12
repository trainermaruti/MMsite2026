# Brand Header Implementation Guide

## Overview
Premium brand header for Maruti Makwana Training Portal using **Plus Jakarta Sans** typography with gradient effects.

---

## Files Created/Modified

### 1. `wwwroot/css/brand.css`
Premium brand styles with:
- Plus Jakarta Sans font (with Inter fallback)
- Gradient text effects
- Responsive design
- Accessibility support
- Theme variants

### 2. `Views/Shared/_Brand.cshtml`
Reusable brand header partial component.

### 3. `Views/Shared/_Layout.cshtml`
Updated to include brand.css and use the brand partial.

### 4. `Tests/BrandHeaderTests.cs`
Unit tests for brand header markup validation.

---

## Quick Start

### Step 1: Update Layout File
In `Views/Shared/_Layout.cshtml`, add the brand.css reference in the `<head>` section:

```html
<link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
<link rel="stylesheet" href="~/css/brand.css" asp-append-version="true" />
```

### Step 2: Replace Existing Brand Markup
In your navbar section of `_Layout.cshtml`, replace the existing navbar-brand anchor with:

```html
@await Html.PartialAsync("_Brand")
```

Example:
```html
<nav class="navbar-modern">
    <div class="container-fluid">
        @await Html.PartialAsync("_Brand")
        <!-- rest of navbar -->
    </div>
</nav>
```

### Step 3: Update Logo Image (MANUAL STEP REQUIRED)
**Action Required:** Ensure your logo file exists at `wwwroot/images/logo.jpg` (or update path in `_Brand.cshtml`).

**Recommended Logo Specifications:**
- Format: PNG with transparency (or JPG)
- Dimensions: 40x40px (or 80x80px @2x for retina)
- File size: < 50KB
- Background: Transparent or matching navbar color

To replace logo:
1. Place new logo at: `wwwroot/images/logo.jpg` (or `logo.png`)
2. If using different filename, update `_Brand.cshtml`:
   ```html
   <img src="~/images/YOUR_LOGO.png" alt="Maruti Makwana logo" />
   ```

---

## Customization

### Change Gradient Colors
Edit CSS variables in `wwwroot/css/brand.css`:

```css
:root {
    --brand-gradient-start: #ffffff;      /* Start color */
    --brand-gradient-mid: #60a5fa;        /* Middle color */
    --brand-gradient-end: #3b82f6;        /* End color */
    --brand-glow-color: rgba(59, 130, 246, 0.4); /* Glow effect */
}
```

**Example - Purple Gradient:**
```css
:root {
    --brand-gradient-start: #ffffff;
    --brand-gradient-mid: #a855f7;
    --brand-gradient-end: #7c3aed;
    --brand-glow-color: rgba(124, 58, 237, 0.4);
}
```

**Example - Green Gradient:**
```css
:root {
    --brand-gradient-start: #ffffff;
    --brand-gradient-mid: #34d399;
    --brand-gradient-end: #10b981;
    --brand-glow-color: rgba(16, 185, 129, 0.4);
}
```

### Light Theme Variant
The brand.css includes a `.light-theme` class. To enable dynamic theme switching:

**1. Add theme toggle to your layout:**
```html
<button id="themeToggle" class="btn btn-sm">
    <i class="fas fa-sun"></i>
</button>
```

**2. Add JavaScript (in `wwwroot/js/site.js` or inline):**
```javascript
document.addEventListener('DOMContentLoaded', function() {
    const themeToggle = document.getElementById('themeToggle');
    const body = document.body;
    
    // Load saved theme
    const savedTheme = localStorage.getItem('theme') || 'dark';
    if (savedTheme === 'light') {
        body.classList.add('light-theme');
    }
    
    // Toggle theme
    themeToggle?.addEventListener('click', function() {
        body.classList.toggle('light-theme');
        const theme = body.classList.contains('light-theme') ? 'light' : 'dark';
        localStorage.setItem('theme', theme);
        
        // Update icon
        const icon = this.querySelector('i');
        icon.classList.toggle('fa-sun');
        icon.classList.toggle('fa-moon');
    });
});
```

### Change Text Content
Edit `Views/Shared/_Brand.cshtml`:

```html
<span class="brand-name-main">YOUR BRAND NAME</span>
<span class="brand-name-sub">Your Tagline</span>
```

### Adjust Font Size
In `brand.css`, modify:

```css
.brand-name-main {
    font-size: 20px;  /* Change this */
    letter-spacing: 1.5px;
}

.brand-name-sub {
    font-size: 11px;  /* Change this */
}
```

---

## Accessibility Features

✅ **Screen Reader Support**
- Alt text on logo image
- Title attribute on link
- Semantic HTML structure

✅ **Keyboard Navigation**
- Focus-visible outline on brand link
- No keyboard traps

✅ **Reduced Motion**
- Animations disabled when `prefers-reduced-motion` is enabled

✅ **High Contrast Mode**
- Fallback to solid white text when `prefers-contrast: high`

✅ **Browser Compatibility**
- Fallback for browsers without `background-clip: text`
- Multiple font fallbacks (Plus Jakarta Sans → Inter → System fonts)

---

## Manual Testing

**Visual Testing:**
1. Start the application: `dotnet run`
2. Navigate to homepage
3. Verify brand header displays correctly

**Markup Validation:**
- Check brand markup in browser DevTools
- Verify CSS classes are applied
- Confirm logo image loads
- Test accessibility with screen reader

---

## Browser Support

| Browser | Version | Support |
|---------|---------|---------|
| Chrome  | 90+     | ✅ Full |
| Firefox | 88+     | ✅ Full |
| Safari  | 14+     | ✅ Full |
| Edge    | 90+     | ✅ Full |
| Opera   | 76+     | ✅ Full |

**Note:** Gradient text effects degrade gracefully to solid white on older browsers.

---

## Troubleshooting

### Logo Not Showing
1. Verify file exists: `wwwroot/images/logo.jpg`
2. Check file permissions
3. Clear browser cache
4. Check browser console for 404 errors

### Gradient Not Visible
1. Ensure `brand.css` is loaded after `site.css`
2. Check browser supports `-webkit-background-clip: text`
3. Verify CSS variables are defined
4. Check for conflicting styles in other CSS files

### Font Not Loading
1. Check network tab for Google Fonts request
2. Verify internet connection
3. Check Content Security Policy (CSP) headers
4. Fallback fonts (Inter, system fonts) should still work

### Tests Failing
1. Ensure partial file exists: `Views/Shared/_Brand.cshtml`
2. Check file paths in test assertions
3. Verify xUnit package is installed
4. Run `dotnet restore` and rebuild

---

## Performance Optimization

### Font Loading
The brand.css uses `display=swap` parameter in Google Fonts URL to prevent FOIT (Flash of Invisible Text).

### Critical CSS (Optional)
For production, consider inlining critical brand styles:

```html
<style>
    @import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@800&display=swap');
    .brand-name-main { font-family: 'Plus Jakarta Sans', sans-serif; font-weight: 800; }
</style>
```

### Image Optimization
Use next-gen formats:
- WebP with PNG fallback
- Compress with tools like TinyPNG or ImageOptim
- Consider SVG for logos (scalable, small file size)

---

## Next Steps

1. ✅ Replace logo file at `wwwroot/images/logo.jpg`
2. ✅ Test brand header in all browsers
3. ✅ Customize colors to match brand identity
4. ✅ Run unit tests
5. ✅ Add theme toggle (optional)
6. ✅ Deploy and verify

---

## Support & Questions

For customization help or issues:
1. Check existing styles in `theme.css` for conflicts
2. Verify CSS variable values
3. Test in browser DevTools
4. Review console for errors

**File Locations:**
- Styles: `wwwroot/css/brand.css`
- Markup: `Views/Shared/_Brand.cshtml`
- Tests: `Tests/BrandHeaderTests.cs`
- Layout: `Views/Shared/_Layout.cshtml`
