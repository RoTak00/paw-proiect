using Microsoft.AspNetCore.Identity;
using PAW_Project.Models;

namespace PAW_Project.ViewModels;

public class AdminPanelViewModel
{
    public double AverageTasksFile { get; set; }
    public Dictionary<string?, int> UsagePerUser { get; set; } = new();
    public Dictionary<string, int> UsagePerTool { get; set; } = new();
    public Dictionary<string?, double> StorageByUser { get; set; } = new();
    public Dictionary<string, double> StorageByTool { get; set; } = new();
    public List<ApplicationUser> Users { get; set; } = new();
    
    public UserManager<ApplicationUser> _userManager { get; set; } = null!;
}