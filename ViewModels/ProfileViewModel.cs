using System.ComponentModel.DataAnnotations;
using PAW_Project.Models;

namespace PAW_Project.ViewModels;

public class ProfileViewModel
{
    public String username { get; set; } = default!;
    public String email { get; set; } = default!;
    
    [Required, DataType(DataType.Password)]
    public String currentPassword { get; set; } = default!;
    
    [Required, DataType(DataType.Password)]
    public String newPassword { get; set; } = default!;
    
    [Required, DataType(DataType.Password)]
    [Compare("newPassword", ErrorMessage = "Passwords do not match")]
    public String confirmPassword { get; set; } = default!;
}