namespace PAW_Project.ViewModels;
using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required] 
    public string Username { get; set; } = default!;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = default!;
}