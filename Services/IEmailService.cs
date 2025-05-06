namespace PAW_Project.Services;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string html_message);
}