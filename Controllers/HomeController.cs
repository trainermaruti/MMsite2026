using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MarutiTrainingPortal.Models;
using MarutiTrainingPortal.Data;
using Microsoft.EntityFrameworkCore;

namespace MarutiTrainingPortal.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.FeaturedVideo = await _context.FeaturedVideos
            .Where(v => v.IsActive)
            .OrderBy(v => v.DisplayOrder)
            .FirstOrDefaultAsync();
            
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
