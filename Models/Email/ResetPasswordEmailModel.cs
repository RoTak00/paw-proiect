namespace PAW_Project.Models.Email;

public class ResetPasswordEmailModel
{
    public string Username { get; set; } = default!;
    public string ResetLink { get; set; } = default!;
}