using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PAW_Project.Data;
using PAW_Project.Models;
using PAW_Project.ViewModels;

namespace PAW_Project.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        bool connected;
        try
        {
            connected = await _context.Database.CanConnectAsync();
        }
        catch (Exception ex)
        {
            connected = false;
            _logger.LogError(ex, "Failed to connect to database...");
        }
        
        ViewBag.ConnectionStatus = connected ? "Connected successfully." : "Failed to connect to database.";

        var model = new HomeViewModel()
        {
            // Create a set of mock tools to be used by the user
            // These will be loaded from the database in the future
            ImageTools = _context.ImageTools.ToList()
        };

        return View(model);
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