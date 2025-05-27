using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAW_Project.Data;
using PAW_Project.Models;
using PAW_Project.ViewModels;
using ZstdSharp.Unsafe;

namespace PAW_Project.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public IActionResult Panel()
    {
        var storageByTool = _context.ImageTasks.
            GroupBy(t => t.ImageTool.Name).
            ToDictionary(g => g.Key, 
                g => g.Sum(f =>
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", f.OutputPath);
                    if (System.IO.File.Exists(path))
                    {
                        return new FileInfo(path).Length / 1024.0 / 1024.0;
                    }
                    return 0;
                }));

        var storageByUser = _context.UploadFiles
            .Where(f => f.User != null).GroupBy(f => f.User.UserName)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(f =>
                {
                    double fileStorage = 0.0;

                    var inputPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", f.FileName);
                    if (System.IO.File.Exists(inputPath))
                    {
                        fileStorage += new FileInfo(inputPath).Length / 1024.0 / 1024.0;
                    }
                    return fileStorage;
                }));

        var taskStorageByUser = _context.ImageTasks
            .Where(t => t.File.User != null).GroupBy(t => t.File.User.UserName)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(t =>
                {
                    double taskStorage = 0.0;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", t.OutputPath);
                    if (System.IO.File.Exists(path))
                    {
                        taskStorage += new FileInfo(path).Length / 1024.0 / 1024.0;
                    }

                    return taskStorage;
                }));

        foreach (var key in storageByUser)
        {
            storageByUser[key.Key] += taskStorageByUser[key.Key];
        }
        
        var usagePerTool = _context.ImageTasks
            .GroupBy(t => t.ImageTool.Name)
            .ToDictionary(g => g.Key,
                g => g.Count());
        
        var usagePerUser = _context.ImageTasks
            .Where(t => t.File.User != null)
            .GroupBy(t => t.File.User.UserName)
            .ToDictionary(g => g.Key,
                g => g.Count());
        
        double AverageTasksFile = _context.ImageTasks.Count() / _context.UploadFiles.Count();
        
        var users = _userManager.Users.ToList();

        AdminPanelViewModel model = new AdminPanelViewModel
        {
            StorageByTool = storageByTool,
            StorageByUser = storageByUser,
            UsagePerTool = usagePerTool,
            UsagePerUser = usagePerUser,
            AverageTasksFile = AverageTasksFile,
            Users = users,
            _userManager = _userManager
        };
        
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null || _userManager.GetRolesAsync(currentUser).Result.Contains("Admin") == false)
        {
            TempData["ErrorMessage"] = "You do not have permission to delete this user.";
            return RedirectToAction("Panel");
        }
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["ErrorMessage"] = "User not found.";
            return RedirectToAction("Panel");
        }
        _context.Users.Remove(user);
        TempData["SuccessMessage"] = "User deleted successfully.";
        await _context.SaveChangesAsync();
        return RedirectToAction("Panel");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUser(string Id, string Name, string Email, string NewPassword,
        string ColorTheme, string Role)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null || _userManager.GetRolesAsync(currentUser).Result.Contains("Admin") == false)
        {
            TempData["ErrorMessage"] = "You do not have permission to delete this user.";
            return RedirectToAction("Panel");
        }
        var user = await _userManager.FindByIdAsync(Id);
        if (user == null) return NotFound();

        user.UserName = Name;
        user.Email = Email;
        user.PreferredTheme = ColorTheme;

        var result = await _userManager.UpdateAsync(user);

        
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = "Error updating user.";
            return RedirectToAction("Panel");
        }

        if (!string.IsNullOrEmpty(NewPassword))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passResult = await _userManager.ResetPasswordAsync(user, token, NewPassword);
            if (!passResult.Succeeded)
            {
                TempData["ErrorMessage"] = "Password could not be reset.";
                return RedirectToAction("Panel");
            }
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (!roles.Contains(Role))
        {
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, Role);
        }

        TempData["SuccessMessage"] = "User updated successfully.";
        return RedirectToAction("Panel");

    }
}