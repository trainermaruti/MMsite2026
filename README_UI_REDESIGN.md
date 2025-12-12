# UI Redesign - Enterprise Elite Implementation

## Files Changed/Created

### New Files
- `wwwroot/css/design-system.css` - Centralized design system variables
- `Models/CourseCardViewModel.cs` - Course card data model
- `Views/Shared/_CourseCard.cshtml` - Reusable course card partial
- `wwwroot/css/admin.css` - Admin panel styling overrides
- `README_UI_REDESIGN.md` - This file
- `QA_CHECKLIST_UI.md` - Quality assurance checklist

### Modified Files
- `Views/Home/Index.cshtml` - New hero section
- `Views/Shared/_Layout.cshtml` - Added design-system.css reference

## How to Test

1. **Run the application**
   ```bash
   dotnet run
   ```

2. **Test homepage**
   - Navigate to `/`
   - Verify new hero section displays correctly
   - Check CTA buttons ("Book a Workshop", "See Courses")
   - Verify stats display (50+ Trainings, 2000+ Students, 30+ Companies)

3. **Test responsive design**
   - Open browser DevTools (F12)
   - Test at 320px (mobile), 768px (tablet), 1200px (desktop)
   - Verify hero portrait masks/fades correctly
   - Check button layout collapses appropriately

4. **Test course cards** (if implemented)
   - Navigate to course listing pages
   - Verify standardized card appearance
   - Check badges, images, and CTA buttons

5. **Test admin pages**
   - Navigate to `/Admin`
   - Verify simplified sidebar and header
   - Check small-box widgets styling
   - Verify table hover effects

## Rollback Steps

### Option 1: Git Revert
```bash
git checkout HEAD -- Views/Home/Index.cshtml
git checkout HEAD -- Views/Shared/_Layout.cshtml
```

### Option 2: Manual Removal
1. Remove `<link>` to `design-system.css` from `_Layout.cshtml`
2. Restore previous hero section in `Views/Home/Index.cshtml`
3. Delete new files: `design-system.css`, `_CourseCard.cshtml`, `CourseCardViewModel.cs`, `admin.css`

### Option 3: Feature Flag (Future)
Wrap new hero in conditional rendering based on configuration flag.

## Image Guidelines

### Hero Portrait
- **Source**: `wwwroot/images/profile/44-1200.webp`
- **Recommended size**: 1200px height, 520-600px width
- **Format**: WebP (optimized for web)
- **Mask**: Linear gradient fade to transparent at bottom 20%
- **Fallback**: Provide JPG version for older browsers

### Export Settings
- WebP quality: 85%
- Resolution: 2x for retina displays (actual 2400px height, scaled to 1200px)
- Color profile: sRGB

## Browser Support

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Performance Notes

- Design system CSS adds ~8KB (gzipped ~3KB)
- Hero gradient effects use GPU-accelerated CSS
- WebP images reduce file size by 30-50% vs JPEG

## Next Steps

1. Implement `_CourseCard` partial across all course listing pages
2. Add Google Fonts link for Plus Jakarta Sans in `_Layout.cshtml`
3. Optimize hero image with responsive srcset
4. Add preload hint for hero portrait
