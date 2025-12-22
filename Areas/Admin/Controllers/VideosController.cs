using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarutiTrainingPortal.Data;
using MarutiTrainingPortal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;

namespace MarutiTrainingPortal.Areas.Admin.Controllers
{
    /// <summary>
    /// Admin controller for managing the complete Video Library.
    /// Ensures newest videos appear first (PublishDate DESC).
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class VideosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _jsonPath;
        private static readonly HttpClient _httpClient;

        static VideosController()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            };
            _httpClient = new HttpClient(handler);
            // Mobile User-Agent often leads to faster page loads
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 10; K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Mobile Safari/537.36");
        }

        public VideosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            // Production-safe path resolution
            _jsonPath = Path.Combine(env.ContentRootPath, "JsonData", "VideosDatabase.json");
        }

        // GET: Admin/Videos
        public async Task<IActionResult> Index()
        {
            // Requirement Step 7: Sorting must be done at the query level
            var videos = await _context.Videos
                .OrderByDescending(v => v.Id)
                .ToListAsync();
            return View(videos);
        }

        // GET: Admin/Videos/Create
        public IActionResult Create()
        {
            return View(new Video { PublishDate = DateTime.Now });
        }

        // POST: Admin/Videos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Video video)
        {
            if (ModelState.IsValid)
            {
                // Auto-generate Thumbnail if missing
                if (string.IsNullOrWhiteSpace(video.ThumbnailUrl))
                {
                    video.ThumbnailUrl = video.GenerateThumbnailUrl();
                }

                video.CreatedDate = DateTime.UtcNow;
                _context.Add(video);
                await _context.SaveChangesAsync();
                
                // Sync to JSON for persistence
                await SyncToJsonAsync();
                
                TempData["SuccessMessage"] = "Video added to library successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(video);
        }

        // GET: Admin/Videos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var video = await _context.Videos.FindAsync(id);
            if (video == null) return NotFound();
            
            return View(video);
        }

        // POST: Admin/Videos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Video video)
        {
            if (id != video.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Auto-generate Thumbnail if missing
                    if (string.IsNullOrWhiteSpace(video.ThumbnailUrl))
                    {
                        video.ThumbnailUrl = video.GenerateThumbnailUrl();
                    }

                    video.UpdatedDate = DateTime.UtcNow;
                    _context.Update(video);
                    await _context.SaveChangesAsync();
                    
                    await SyncToJsonAsync();
                    
                    TempData["SuccessMessage"] = "Video updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideoExists(video.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(video);
        }

        // GET: Admin/Videos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var video = await _context.Videos.FirstOrDefaultAsync(m => m.Id == id);
            if (video == null) return NotFound();

            return View(video);
        }

        // POST: Admin/Videos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var video = await _context.Videos.FindAsync(id);
            if (video != null)
            {
                // Soft Delete implementation
                video.IsDeleted = true;
                video.IsActive = false;
                video.UpdatedDate = DateTime.UtcNow;
                _context.Update(video);
                await _context.SaveChangesAsync();
                
                await SyncToJsonAsync();
                TempData["SuccessMessage"] = "Video removed from library.";
            }
            
            return RedirectToAction(nameof(Index));
        }

        // AJAX POST: Admin/Videos/SyncToJson
        [HttpPost]
        public async Task<IActionResult> SyncToJson()
        {
            try
            {
                await SyncToJsonAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // AJAX POST: Admin/Videos/ImportFromJson
        [HttpPost]
        public async Task<IActionResult> ImportFromJson()
        {
            try
            {
                // This is a specialized import for Admin portal
                // For simplicity, we can rely on the existing importer or implement a quick reread here
                // However, the pattern in Trainings seems to be a reload of internal state.
                // Since this is an In-Memory DB, we might need to call into the JsonDataImporter 
                // but usually the DB is the source of truth once running.
                // Let's implement a "reset to JSON" logic.
                
                if (System.IO.File.Exists(_jsonPath))
                {
                    var jsonData = await System.IO.File.ReadAllTextAsync(_jsonPath);
                    var videos = JsonSerializer.Deserialize<List<Video>>(jsonData, new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    });

                    if (videos != null)
                    {
                        // Remove existing non-deleted videos
                        var existing = await _context.Videos.ToListAsync();
                        _context.Videos.RemoveRange(existing);
                        
                        // Add from JSON
                        foreach(var v in videos)
                        {
                            v.Id = 0; // Let EF handle IDs for new records if needed, 
                                     // but here we want to restore state.
                            _context.Videos.Add(v);
                        }
                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                }
                return NotFound("JSON file not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // AJAX GET: Admin/Videos/GetVideoMetadata?url=...
        [HttpGet]
        public async Task<IActionResult> GetVideoMetadata(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return BadRequest("URL is required.");

            try
            {
                // Reuse the static _httpClient for performance
                var html = await _httpClient.GetStringAsync(url);

                // Extract Title (Proactive check for various tags)
                string title = "";
                var titleMatch = Regex.Match(html, "<meta property=\"og:title\" content=\"([^\"]+)\">");
                if (!titleMatch.Success) titleMatch = Regex.Match(html, "<meta name=\"twitter:title\" content=\"([^\"]+)\">");
                if (!titleMatch.Success) titleMatch = Regex.Match(html, "<meta name=\"title\" content=\"([^\"]+)\">");
                
                if (titleMatch.Success)
                {
                    title = System.Net.WebUtility.HtmlDecode(titleMatch.Groups[1].Value);
                }
                else
                {
                    var fallbackMatch = Regex.Match(html, "<title>([^<]+)</title>");
                    if (fallbackMatch.Success)
                    {
                        title = System.Net.WebUtility.HtmlDecode(fallbackMatch.Groups[1].Value).Replace(" - YouTube", "");
                    }
                }

                // Extract Date and format to yyyy-MM-dd for HTML5 date input
                string dateStr = "";
                var dateMatch = Regex.Match(html, "<meta itemprop=\"datePublished\" content=\"([^\"]+)\">");
                if (dateMatch.Success)
                {
                    dateStr = dateMatch.Groups[1].Value;
                    if (dateStr.Contains("T"))
                    {
                        dateStr = dateStr.Split('T')[0];
                    }
                }

                // Extract Full Description (Look for ytInitialPlayerResponse JSON blob)
                string description = "";
                // Better regex to handle escaped quotes: "shortDescription":"((?:\\\"|[^"])*)"
                var fullDescMatch = Regex.Match(html, "\"shortDescription\":\"((?:\\\\\"|[^\"])*)\"");
                if (fullDescMatch.Success)
                {
                    description = Regex.Unescape(fullDescMatch.Groups[1].Value);
                }
                else
                {
                    var descMatch = Regex.Match(html, "<meta name=\"description\" content=\"([^\"]+)\">");
                    if (descMatch.Success)
                    {
                        description = System.Net.WebUtility.HtmlDecode(descMatch.Groups[1].Value);
                    }
                }

                return Json(new { title, publishDate = dateStr, description });
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to fetch metadata: {ex.Message}");
            }
        }

        private async Task SyncToJsonAsync()
        {
            try
            {
                var videos = await _context.Videos
                    .Where(v => !v.IsDeleted)
                    .OrderByDescending(v => v.PublishDate)
                    .ToListAsync();

                var json = JsonSerializer.Serialize(videos, new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true
                });

                await System.IO.File.WriteAllTextAsync(_jsonPath, json);
                Console.WriteLine($"✅ Synced {videos.Count} videos to {_jsonPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Sync failed: {ex.Message}");
            }
        }

        private bool VideoExists(int id)
        {
            return _context.Videos.Any(e => e.Id == id);
        }
    }
}
