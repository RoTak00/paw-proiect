namespace PAW_Project.Services;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
    Task SendTemplatedEmailAsync<TModel>(string to, string subject, string viewName, TModel model);

}