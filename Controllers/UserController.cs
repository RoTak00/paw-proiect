using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PAW_Project.Data;
using PAW_Project.Models;
using PAW_Project.ViewModels;

namespace PAW_Project.Controllers;


[Authorize]
public class UserController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserController> _logger;
    private readonly AppDbContext _context;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public UserController(UserManager<ApplicationUser> userManager, ILogger<UserController> logger, AppDbContext context, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _logger = logger;
        _context = context;
        _signInManager = signInManager;
    }
    public IActionResult Profile()
    {
        var user = _userManager.GetUserAsync(User).Result;
        var model = new ProfileViewModel
        {
            username = user.UserName,
            email = user.Email
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            user.UserName = model.username;
            user.Email = model.email;
            await _userManager.UpdateAsync(user);
            TempData["SuccessMessage"] = "Name/Email changed successfully!";
        }
        return RedirectToAction("Profile");
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Profile", new { fragment = "changePassword" });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var result = await _userManager.ChangePasswordAsync(user, model.currentPassword, model.newPassword);
        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction("Profile"); 
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return RedirectToAction("Profile", model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAccount()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Delete the user
        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "Your account has been deleted successfully.";
            return RedirectToAction("Index", "Home");
        }

        // If deletion failed, show errors
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        TempData["ErrorMessage"] = "There was a problem deleting your account.";
        return RedirectToAction("Profile", new { section = "deleteAccount" });
    }
    
    public async Task<IActionResult> MyImages()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {

            var uploaded = await _context.UploadFiles
                .Where(f => f.UserId == user.Id)
                .ToListAsync();

            var tasks = await _context.ImageTasks
                .Include(t => t.File)
                .Include(t => t.ImageTool)
                .Where(t => t.File.UserId == user.Id)
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
        var user = await _userManager.GetUserAsync(User);
        if (file != null)
        {
            if (user == null || file.UserId != user.Id)
            {
                TempData["ErrorMessage"] = "You do not have permission to delete this file.";
                return RedirectToAction("MyImages");
            }
            var inputPath = Path.Combine("wwwroot/uploads/", file.FileName);
            var outputPaths = _context.ImageTasks
                .Where(t => t.FileId == file.Id)
                .Select(t => t.OutputPath)
                .ToList();
            _context.UploadFiles.Remove(file);
            await _context.SaveChangesAsync();
            foreach (var path in outputPaths)
            {
                var complete = "./wwwroot" + path;
                if (System.IO.File.Exists(complete))
                {
                    System.IO.File.Delete(complete);
                }
            }
            if (System.IO.File.Exists(inputPath))
            {
                System.IO.File.Delete(inputPath);
            }
        }
        return RedirectToAction("MyImages");
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.ImageTasks.FindAsync(id);
        if (task != null)
        {
            var baseImage = await _context.UploadFiles.FindAsync(task.FileId);
            var user = await _userManager.GetUserAsync(User);
            if (user == null || baseImage == null || baseImage.UserId != user.Id)
            {
                TempData["ErrorMessage"] = "You do not have permission to delete this file.";
                return RedirectToAction("MyImages");
            }

            var path = "./wwwroot" + task.OutputPath;
            _context.ImageTasks.Remove(task);
            await _context.SaveChangesAsync();
            Console.WriteLine(path);
            // Delete the file from the server
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            TempData["SuccessMessage"] = "Task deleted successfully.";
            
        }
        return RedirectToAction("MyImages");
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveTheme([FromBody] ThemeDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            user.PreferredTheme = dto.Theme;
            await _userManager.UpdateAsync(user);
        }
        return Ok();
    }

    public class ThemeDto
    {
        public string Theme { get; set; }
    }
}