# QA Checklist - Enterprise Elite UI Redesign

## Visual Checks

### Hero Section
- [ ] Hero section displays on homepage
- [ ] Heading "Azure & AI Training for Enterprise Teams" visible
- [ ] "Enterprise Teams" text shows gradient effect (white to blue)
- [ ] Badge "Available for Corporate Training" displays with checkmark icon
- [ ] Lead paragraph text visible and readable
- [ ] Two CTA buttons present: "Book a Workshop" and "See Courses"
- [ ] Stats row displays: 50+ Trainings, 3000+ Students, 30+ Companies
- [ ] Stats separated by vertical borders

### Hero Portrait
- [ ] Portrait image loads (44-1200.webp)
- [ ] Image has gradient mask/fade at bottom
- [ ] Purple glow effect visible behind portrait
- [ ] Image positioned correctly on right side (desktop)

### Buttons
- [ ] Primary button has blue gradient background
- [ ] Primary button has shadow
- [ ] Outline button has white border
- [ ] Hover states work (transform, color changes)
- [ ] Buttons link to correct pages

## Responsive Design

### Desktop (1200px+)
- [ ] Hero layout uses 2 columns (copy left, portrait right)
- [ ] Stats display inline with borders
- [ ] Buttons display side-by-side
- [ ] Typography scales correctly

### Tablet (768px - 1199px)
- [ ] Hero maintains 2-column layout
- [ ] Stats may wrap but remain readable
- [ ] Portrait scales proportionally

### Mobile (320px - 767px)
- [ ] Hero switches to single column (copy stacks above portrait)
- [ ] Heading font size reduces to 2.5rem
- [ ] Stats stack vertically or wrap
- [ ] Buttons stack vertically or wrap with gap
- [ ] Section padding reduces to 48px

## Course Cards (if implemented)

- [ ] Cards use `_CourseCard.cshtml` partial
- [ ] Card background is elevated surface color (#161616)
- [ ] Image displays with 16:9 ratio
- [ ] Badges show duration and level
- [ ] Title displays in white, bold
- [ ] Summary text is muted color
- [ ] "View Details" button spans full width
- [ ] Cards have consistent height in grid

## Admin Panel

- [ ] Sidebar background is #1e293b (no gradients)
- [ ] Header background matches sidebar
- [ ] Small-box widgets have #334155 background
- [ ] Small-box icons are subtle (10% white opacity)
- [ ] Table headers have dark background (#1e293b)
- [ ] Table rows have hover effect (2% white overlay)

## Accessibility

### Contrast Ratios (WCAG AA)
- [ ] Body text (#e5e5e5 on #0a0a0a) meets 4.5:1 minimum
- [ ] Heading text (white on dark) meets 7:1 enhanced
- [ ] Button text has sufficient contrast
- [ ] Muted text (#737373) readable against background

### Keyboard Navigation
- [ ] All buttons focusable with Tab key
- [ ] Focus indicators visible
- [ ] CTA buttons activate with Enter/Space

### Screen Readers
- [ ] Image alt text present ("Maruti Makwana, Corporate Azure Trainer")
- [ ] Headings use proper hierarchy (h1, h2, h3)
- [ ] Stats have descriptive labels

## Browser Compatibility

- [ ] Chrome: Hero displays correctly
- [ ] Firefox: Gradient mask works
- [ ] Safari: WebP image loads (or fallback)
- [ ] Edge: All animations smooth

## Performance

### Lighthouse Audit
- [ ] Performance score: 90+ (mobile)
- [ ] Performance score: 95+ (desktop)
- [ ] First Contentful Paint: < 1.5s
- [ ] Largest Contentful Paint: < 2.5s
- [ ] Cumulative Layout Shift: < 0.1

### Network
- [ ] design-system.css loads (check Network tab)
- [ ] Hero portrait image loads
- [ ] No 404 errors in console
- [ ] Total page weight: < 1MB (without video)

## Functional Tests

- [ ] "Book a Workshop" button navigates to /Contact
- [ ] "See Courses" button navigates to /Courses
- [ ] No JavaScript console errors
- [ ] No CSS warnings in DevTools
- [ ] Page loads without layout shifts

## Cross-Browser Testing

### Chrome
- [ ] Hero section renders correctly
- [ ] Gradient text effect displays
- [ ] Buttons have hover animations

### Firefox
- [ ] All styles match Chrome
- [ ] Mask-image gradient works on portrait

### Safari (macOS/iOS)
- [ ] WebP image displays (or fallback)
- [ ] -webkit-text-fill-color works for gradient text
- [ ] Backdrop filters work (if used)

### Edge
- [ ] Chromium-based Edge matches Chrome behavior

## Final Checks

- [ ] No broken images (check Network tab)
- [ ] Fonts load correctly (Plus Jakarta Sans or fallback)
- [ ] Design system CSS variables apply globally
- [ ] Admin CSS overrides work without conflicts
- [ ] Rollback procedure documented and tested
- [ ] Git commit contains all changed files
- [ ] Build completes without warnings: `dotnet build`
