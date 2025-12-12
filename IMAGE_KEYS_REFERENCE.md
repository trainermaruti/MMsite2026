# Website Image Keys - Quick Reference

This file lists all image keys used throughout the website. Use these keys when updating images through the admin panel.

## Current Image Keys

### Profile Images
- **`profile_main`** - Main profile picture / DP (Display Picture)
  - Used in: Header navigation, Hero section, **Admin panel avatar dropdown**
  - Current: `/images/44.png`
  - Size recommendation: 500x500px (square)
  - Category: Profile
  - **Special Note**: This is the main DP that appears throughout the site including admin panel

- **`profile_hero`** - Large about section profile photo
  - Used in: About section main image
  - Current: `/images/22.png`
  - Size recommendation: 420x560px (portrait)
  - Category: About

### Badge Images
- **`experience_badge`** - Experience years badge
  - Used in: About section overlay
  - Current: `/images/experience-badge.png`
  - Size recommendation: 180x180px
  - Category: Badge
  - Note: Positioned top-left of about image

---

## Adding New Image Keys

When adding new images to the website:

1. Choose a descriptive key following the pattern: `section_purpose`
2. Add through admin panel: `/Admin/WebsiteImages/Create`
3. Update this document with the new key
4. Use in views with: `@await ImageService.GetImageUrlAsync("your_key")`

### Suggested Keys for Future Images

These are placeholder suggestions for when you need to add more images:

#### Header/Navigation
- `logo_header` - Company/brand logo in header
- `logo_footer` - Company/brand logo in footer
- `favicon` - Browser tab icon

#### Hero Section
- `hero_background` - Background image for hero
- `hero_overlay` - Overlay pattern/texture

#### About Section
- `about_background` - Background for about section
- `skill_icon_1` through `skill_icon_n` - Individual skill icons
- `certification_1` through `certification_n` - Certification badges

#### Training Cards
- `training_azure_bg` - Azure training card background
- `training_ai_bg` - AI training card background
- `training_data_bg` - Data Science card background

#### Testimonials
- `testimonial_company_1` - Client company logo
- `testimonial_photo_1` - Client photo

#### Contact Section
- `contact_map` - Office location map
- `contact_bg` - Background image

#### Social Media
- `social_linkedin_icon` - LinkedIn icon (if custom)
- `social_youtube_icon` - YouTube icon (if custom)
- `social_instagram_icon` - Instagram icon (if custom)

---

## Image Size Guidelines

For best results, use these dimensions:

| Image Type | Recommended Size | Aspect Ratio |
|-----------|------------------|--------------|
| Profile Square | 500x500px | 1:1 |
| Profile Portrait | 420x560px | 3:4 |
| Badges | 180x180px | 1:1 |
| Hero Background | 1920x1080px | 16:9 |
| Card Background | 800x600px | 4:3 |
| Logos | 400x100px | 4:1 |
| Icons | 64x64px | 1:1 |

## Format Guidelines

- **Photos**: JPG or WEBP (better compression)
- **Graphics/Logos**: PNG (with transparency) or SVG
- **Icons**: SVG preferred for scalability
- **Backgrounds**: JPG or WEBP, optimized for web

## Optimization Tips

1. Compress images before upload (use TinyPNG, Squoosh, etc.)
2. Use appropriate format for content type
3. Keep file sizes under 500KB for fast loading
4. Use lazy loading for below-the-fold images
5. Consider providing 2x versions for retina displays

---

**Last Updated:** Current session
**Maintained by:** Admin team
**Questions:** Refer to README_IMAGE_MANAGEMENT.md
