using MarutiTrainingPortal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Xml;

namespace MarutiTrainingPortal.Controllers
{
    /// <summary>
    /// Generates dynamic sitemap.xml from database content for SEO.
    /// Includes all public pages: trainings, courses, events, static pages.
    /// </summary>
    public class SitemapController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SitemapController> _logger;

        public SitemapController(ApplicationDbContext context, ILogger<SitemapController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("sitemap.xml")]
        [ResponseCache(Duration = 3600)] // Cache for 1 hour
        public async Task<IActionResult> Index()
        {
            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var sitemap = await GenerateSitemapAsync(baseUrl);

                return Content(sitemap, "application/xml", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating sitemap");
                return StatusCode(500);
            }
        }

        private async Task<string> GenerateSitemapAsync(string baseUrl)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                Encoding = Encoding.UTF8,
                Async = true
            };

            using var stream = new MemoryStream();
            using (var writer = XmlWriter.Create(stream, settings))
            {
                await writer.WriteStartDocumentAsync();
                await writer.WriteStartElementAsync(null, "urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                // Static pages
                await AddUrlAsync(writer, baseUrl, "/", "daily", "1.0");
                await AddUrlAsync(writer, baseUrl, "/Trainings", "weekly", "0.8");
                await AddUrlAsync(writer, baseUrl, "/Courses", "weekly", "0.8");
                await AddUrlAsync(writer, baseUrl, "/Events", "daily", "0.9");
                await AddUrlAsync(writer, baseUrl, "/Events/Calendar", "daily", "0.9");
                await AddUrlAsync(writer, baseUrl, "/Profile/About", "monthly", "0.7");
                await AddUrlAsync(writer, baseUrl, "/FAQ", "monthly", "0.7");
                await AddUrlAsync(writer, baseUrl, "/Contact", "monthly", "0.6");
                await AddUrlAsync(writer, baseUrl, "/Verify", "monthly", "0.7");

                // Dynamic training pages
                var trainings = await _context.Trainings
                    .OrderByDescending(t => t.DeliveryDate)
                    .ToListAsync();

                foreach (var training in trainings)
                {
                    var lastMod = training.UpdatedDate ?? training.CreatedDate;
                    await AddUrlAsync(writer, baseUrl, $"/Trainings/Details/{training.Id}", "monthly", "0.6", lastMod);
                }

                // Dynamic course pages
                var courses = await _context.Courses
                    .OrderByDescending(c => c.PublishedDate)
                    .ToListAsync();

                foreach (var course in courses)
                {
                    var lastMod = course.UpdatedDate ?? course.CreatedDate;
                    await AddUrlAsync(writer, baseUrl, $"/Courses/Details/{course.Id}", "monthly", "0.6", lastMod);
                }

                // Dynamic event pages
                var events = await _context.TrainingEvents
                    .Where(e => e.StartDate > DateTime.UtcNow.AddMonths(-1)) // Only recent/upcoming events
                    .OrderByDescending(e => e.StartDate)
                    .ToListAsync();

                foreach (var evt in events)
                {
                    var lastMod = evt.UpdatedDate ?? evt.CreatedDate;
                    await AddUrlAsync(writer, baseUrl, $"/Events/Details/{evt.Id}", "weekly", "0.7", lastMod);
                }

                await writer.WriteEndElementAsync(); // urlset
                await writer.WriteEndDocumentAsync();
                await writer.FlushAsync();
            }

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private async Task AddUrlAsync(
            XmlWriter writer,
            string baseUrl,
            string path,
            string changeFreq,
            string priority,
            DateTime? lastMod = null)
        {
            await writer.WriteStartElementAsync(null, "url", null);
            
            await writer.WriteElementStringAsync(null, "loc", null, $"{baseUrl}{path}");
            
            if (lastMod.HasValue)
            {
                await writer.WriteElementStringAsync(null, "lastmod", null, lastMod.Value.ToString("yyyy-MM-dd"));
            }
            
            await writer.WriteElementStringAsync(null, "changefreq", null, changeFreq);
            await writer.WriteElementStringAsync(null, "priority", null, priority);
            
            await writer.WriteEndElementAsync(); // url
        }
    }
}
