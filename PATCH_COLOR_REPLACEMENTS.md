# Color Replacement Migration Guide
**Maruti Makwana Training Portal - Theme System Integration**

## ⚠️ SAFETY FIRST

1. **Create a git branch** before making changes: `git checkout -b theme-migration`
2. **Run tests** after each batch of replacements
3. **Visual QA** header, cards, forms, buttons after changes
4. **Exclude** these directories:
   - `wwwroot/lib/` (vendor libraries)
   - `node_modules/` (if present)
   - `*.min.css` (minified files)
   - Image files and binary assets

## Priority Order

### PRIORITY 1: Body & Surface Backgrounds

| Hardcoded | Replace With | Files |
|-----------|--------------|-------|
| `background: #0f172a` | `background: var(--bg-body-main)` | site.css, theme.css |
| `background-color: #0a0a0a` | `background-color: var(--bg-body-main)` | site.css |
| `background: #1e293b` | `background: var(--bg-surface-solid)` | modern-design-system.css |
| `background: rgba(30,41,59,0.7)` | `background: var(--bg-surface-glass)` | hero-about-utilities.css |

**VSCode Regex Find/Replace:**
```
Find:    background(-color)?: #0f172a;?
Replace: background$1: var(--bg-body-main);
```

**sed command (Linux/macOS):**
```bash
sed -i 's/background: #0f172a/background: var(--bg-body-main)/g' wwwroot/css/site.css
```

---

### PRIORITY 2: Text Colors

| Hardcoded | Replace With | Files |
|-----------|--------------|-------|
| `color: #ffffff` | `color: var(--text-primary)` | site.css, brand.css |
| `color: #cbd5e1` | `color: var(--text-body)` | theme.css |
| `color: #94a3b8` | `color: var(--text-muted)` | modern-design-system.css |
| `color: #a3a3a3` | `color: var(--text-muted)` | hero-about-utilities.css |

**VSCode Regex Find/Replace:**
```
Find:    color: #ffffff;?
Replace: color: var(--text-primary);
```

**sed command:**
```bash
sed -i 's/color: #ffffff/color: var(--text-primary)/g' wwwroot/css/site.css
```

---

### PRIORITY 3: Borders

| Hardcoded | Replace With | Files |
|-----------|--------------|-------|
| `border: 1px solid rgba(255,255,255,0.1)` | `border: 1px solid var(--border-subtle)` | All CSS |
| `border-color: rgba(156,163,175,0.2)` | `border-color: var(--border-subtle)` | brand.css |
| `border: 1px solid rgba(255,255,255,0.2)` | `border: 1px solid var(--border-strong)` | All CSS |

**VSCode Regex Find/Replace:**
```
Find:    border: 1px solid rgba\(255,\s?255,\s?255,\s?0\.1\);?
Replace: border: 1px solid var(--border-subtle);
```

---

### PRIORITY 4: Input Backgrounds

| Hardcoded | Replace With | Files |
|-----------|--------------|-------|
| `background-color: #1a1a1a` | `background-color: var(--bg-input)` | site.css |
| `background: rgba(255,255,255,0.05)` | `background: var(--bg-input)` | modern-design-system.css |

**VSCode Regex:**
```
Find:    background(-color)?: rgba\(255,\s?255,\s?255,\s?0\.05\);?
Replace: background$1: var(--bg-input);
```

---

### PRIORITY 5: Shadows

| Hardcoded | Replace With | Files |
|-----------|--------------|-------|
| `box-shadow: 0 10px 30px -10px rgba(167,139,250,0.3)` | `box-shadow: var(--shadow-elevation)` | All CSS |
| Complex multi-layer shadows | `box-shadow: var(--shadow-elevation)` | Replace case-by-case |

**Manual Replacement** (too varied for regex)

---

### PRIORITY 6: Brand Accent Colors

| Hardcoded | Replace With | Files |
|-----------|--------------|-------|
| `#a78bfa` | `var(--accent-purple)` | All CSS |
| `#22d3ee` | `var(--accent-cyan)` | All CSS |
| `#60a5fa`, `#3b82f6` (blues) | Keep gradient, or use `var(--accent-purple)` | Case-by-case |

---

## Step-by-Step Process

### Step 1: Backup
```bash
git add .
git commit -m "Pre-theme-migration snapshot"
git checkout -b theme-migration
```

### Step 2: Replace in Batches

```bash
# Priority 1: Body backgrounds
find wwwroot/css -name "*.css" ! -name "*.min.css" -exec sed -i 's/background: #0f172a/background: var(--bg-body-main)/g' {} +

# Priority 2: Text colors
find wwwroot/css -name "*.css" ! -name "*.min.css" -exec sed -i 's/color: #ffffff/color: var(--text-primary)/g' {} +

# Continue for each priority...
```

### Step 3: Test After Each Batch

```bash
dotnet build
dotnet test
# Visit http://localhost:5204 and toggle theme
```

### Step 4: Visual QA Checklist

- [ ] Header/navbar renders correctly (dark & light)
- [ ] Cards have proper backgrounds and borders
- [ ] Form inputs are readable
- [ ] Buttons have correct colors
- [ ] Text contrast is sufficient (use browser DevTools)
- [ ] Hover states work
- [ ] Focus rings visible

### Step 5: Contrast Check

Use browser DevTools (Lighthouse) to verify WCAG AA compliance:
- Text contrast ratio ≥ 4.5:1
- Large text ≥ 3:1

---

## Files to Review Manually

1. `wwwroot/css/site.css` - Override hell, many `!important` flags
2. `wwwroot/css/hero-about-utilities.css` - Inline gradient colors
3. `Views/Home/_HeroSection.cshtml` - Inline styles in HTML
4. `wwwroot/css/modern-design-system.css` - Complex component styles

---

## Rollback Plan

```bash
git checkout main
# Or to undo specific file:
git checkout main -- wwwroot/css/site.css
```

---

## Post-Migration Cleanup

1. Remove unused color variables from old theme.css
2. Consolidate duplicate CSS rules
3. Run CSS linter: `npx stylelint "wwwroot/css/**/*.css"`
4. Minify for production: Consider CSS purge/minification
