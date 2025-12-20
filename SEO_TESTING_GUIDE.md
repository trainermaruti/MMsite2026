# SEO Testing & Verification Guide

## Quick Test Checklist

### 1. View Page Source (Right-click → View Page Source)

#### Homepage Check
```html
<!-- Look for these in <head> section -->
<title>Expert Industrial Automation & PLC Training - Maruti Makwana Training Portal</title>
<meta name="description" content="Join 130,000+ professionals trained..." />
<meta name="keywords" content="PLC training courses, SCADA programming..." />

<!-- Open Graph Tags -->
<meta property="og:type" content="website" />
<meta property="og:title" content="Expert Industrial Automation & PLC Training..." />

<!-- Schema.org Structured Data -->
<script type="application/ld+json">
{
  "@context": "https://schema.org",
  "@type": "EducationalOrganization",
  ...
}
</script>
```

### 2. Test robots.txt
**URL:** `https://marutimakwana.com/robots.txt`
```
User-agent: *
Allow: /
Disallow: /Admin/
Sitemap: https://marutimakwana.com/sitemap.xml
```

### 3. Test Sitemap
**URL:** `https://marutimakwana.com/sitemap.xml`
Should return XML with all public pages listed.

### 4. Social Media Preview Tests

#### Facebook Sharing Debugger
**URL:** https://developers.facebook.com/tools/debug/
- Enter: `https://marutimakwana.com`
- Click "Debug" to see Open Graph preview
- Should show title, description, and logo image

#### Twitter Card Validator
**URL:** https://cards-dev.twitter.com/validator
- Enter: `https://marutimakwana.com`
- Should show summary_large_image card with title/description

#### LinkedIn Post Inspector
**URL:** https://www.linkedin.com/post-inspector/
- Enter: `https://marutimakwana.com`
- Check preview looks professional

### 5. Google Rich Results Test
**URL:** https://search.google.com/test/rich-results
- Enter: `https://marutimakwana.com`
- Should detect "EducationalOrganization" schema
- Verify rating, founder info appears

---

## SEO Tools to Use

### Free Tools
1. **Google Search Console** - https://search.google.com/search-console
   - Add property: `https://marutimakwana.com`
   - Submit sitemap: `https://marutimakwana.com/sitemap.xml`
   - Monitor indexing status

2. **Google PageSpeed Insights** - https://pagespeed.web.dev/
   - Test performance and SEO scores
   - Get mobile/desktop optimization tips

3. **Bing Webmaster Tools** - https://www.bing.com/webmasters
   - Submit sitemap for Bing indexing
   - Verify robots.txt

### Paid Tools (Optional)
- **SEMrush** - Keyword tracking, backlinks, competitor analysis
- **Ahrefs** - Comprehensive SEO audit
- **Moz Pro** - Domain authority tracking

---

## Browser Testing

### Check Meta Tags (Chrome DevTools)
1. Press `F12` to open DevTools
2. Go to **Elements** tab
3. Find `<head>` section
4. Verify all meta tags are present:
   - `<meta name="description">`
   - `<meta property="og:*">`
   - `<meta name="twitter:*">`
   - `<script type="application/ld+json">` for schema

### Console Verification
Open browser console and run:
```javascript
// Check meta description
document.querySelector('meta[name="description"]').content

// Check Open Graph title
document.querySelector('meta[property="og:title"]').content

// Check canonical URL
document.querySelector('link[rel="canonical"]').href

// Check structured data
JSON.parse(document.querySelector('script[type="application/ld+json"]').textContent)
```

---

## Expected Search Results

### When searching "Maruti Makwana PLC training"
**Title:** Expert Industrial Automation & PLC Training - Maruti Makwana...
**Description:** Join 130,000+ professionals trained by Maruti Makwana. Expert training in PLC Programming, SCADA, HMI, Robotics, Industrial Automation...
**URL:** https://marutimakwana.com

### Rich Snippet Elements (may take 2-4 weeks to appear)
- ⭐ 4.8 star rating (from AggregateRating schema)
- 👤 Founded by Maruti Makwana
- 📍 Location indicator
- 🔗 Sitelinks to main pages

---

## Keyword Rankings to Track

### Primary Keywords (Target Top 10)
- PLC training
- SCADA training
- Industrial automation courses
- Maruti Makwana trainer
- HMI programming training

### Secondary Keywords (Target Top 20)
- Automation engineer courses
- PLC programming certification
- Siemens PLC training
- Robotics training courses
- Industrial IoT training

### Location-Based (if applicable)
- PLC training in [Your City]
- Industrial automation training near me
- Best automation trainer in [Your Region]

---

## Post-Deployment Actions

### Week 1
- [ ] Submit sitemap to Google Search Console
- [ ] Submit sitemap to Bing Webmaster Tools
- [ ] Verify robots.txt is accessible
- [ ] Test all meta tags in page source
- [ ] Test social media previews

### Week 2-4
- [ ] Monitor Google Search Console for indexing
- [ ] Check for crawl errors
- [ ] Verify pages are appearing in search
- [ ] Monitor search impressions/clicks

### Month 2-3
- [ ] Track keyword rankings
- [ ] Analyze which keywords drive traffic
- [ ] Update underperforming pages
- [ ] Add more content if needed

### Ongoing
- [ ] Weekly Search Console check
- [ ] Monthly keyword ranking review
- [ ] Quarterly meta description optimization
- [ ] Regular content updates

---

## Troubleshooting

### Meta Tags Not Showing
**Issue:** View source doesn't show SEO tags
**Fix:** 
1. Hard refresh browser (Ctrl+Shift+R)
2. Clear browser cache
3. Verify build was successful: `dotnet build`
4. Check _Layout.cshtml has the meta tags

### Social Preview Not Updating
**Issue:** Old preview shows when sharing
**Fix:**
1. Use Facebook Debug Tool to scrape new data
2. Wait 24-48 hours for cache to clear
3. Verify Open Graph tags in page source

### Google Not Indexing
**Issue:** Pages not appearing in Google search
**Fix:**
1. Verify robots.txt allows crawling
2. Submit sitemap in Search Console
3. Request indexing for specific URLs
4. Wait 1-4 weeks for full indexing

### Keywords Not Ranking
**Issue:** Not appearing for target keywords
**Fix:**
1. Use more specific long-tail keywords
2. Add more quality content
3. Build backlinks from relevant sites
4. Ensure keywords appear in title, H1, first paragraph

---

## Success Metrics

### Immediate (Week 1)
✅ All meta tags present in page source
✅ robots.txt accessible
✅ Sitemap.xml generating correctly
✅ Social media previews working

### Short-term (Month 1-2)
✅ Indexed in Google (site:marutimakwana.com)
✅ Appearing for brand name searches
✅ Rich snippets showing in results
✅ Social sharing working properly

### Long-term (Month 3-6)
✅ Ranking for primary keywords (top 20)
✅ Organic traffic increasing
✅ Click-through rate improving
✅ Conversion rate from organic traffic

---

## Contact for SEO Support

If you need help with SEO after implementation:
- **Google Search Console:** https://support.google.com/webmasters
- **Schema.org Documentation:** https://schema.org/docs/gs.html
- **SEO Community:** https://www.reddit.com/r/SEO/

---

**Last Updated:** December 20, 2025
**Implementation Status:** ✅ Complete
**Next Review:** Weekly for first month, then monthly
