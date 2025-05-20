using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PAW_Project.Data;
using PAW_Project.ViewModels;

namespace PAW_Project.Controllers;


[Authorize]
public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly AppDbContext _context;

    public UserController(ILogger<UserController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public IActionResult Profile()
    {
        return View();
    }
    
    public async Task<IActionResult> MyImages()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var uploaded = await _context.UploadFiles
                .Where(f => f.UserId == userId)
                .ToListAsync();

            var tasks = await _context.ImageTasks
                .Include(t => t.File)
                .Include(t => t.ImageTool)
                .Where(t => t.File.UserId == userId)
                .ToListAsync();

            var grouped = tasks
                .GroupBy(t => t.ImageTool.Name)
                .ToDictionary(g => g.Key, g => g.ToList());

            var model = new MyImagesViewModel
            {
                UploadedFiles = uploaded,
                TasksByTool = grouped
            };

            return View(model);
        }
        return RedirectToAction("Index", "Home");
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteUpload(int id)
    {
        var file = await _context.UploadFiles.FindAsync(id);
        if (file != null)
        {
            _context.UploadFiles.Remove(file);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("MyImages");
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.ImageTasks.FindAsync(id);
        if (task != null)
        {
            _context.ImageTasks.Remove(task);
        }
        return RedirectToAction("MyImages");
    }
}