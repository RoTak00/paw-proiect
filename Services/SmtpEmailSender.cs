using System.Net;
using System.Net.Mail;
using RazorLight;

namespace PAW_Project.Services;

public class SmtpEmailSender : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly RazorLightEngine _engine;

    public SmtpEmailSender(IConfiguration config)
    {
        _configuration = config;
        _engine = new RazorLightEngineBuilder()
            .UseFileSystemProject(Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates"))
            .UseMemoryCachingProvider()
            .Build();

    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var smtp = _configuration.GetSection("Smtp");

        var client = new SmtpClient(smtp.GetValue<string>("Host"), Convert.ToInt32(smtp.GetValue<int>("Port")));
        client.EnableSsl = Convert.ToBoolean(smtp.GetValue<bool>("EnableSsl"));
        client.Credentials = new NetworkCredential(smtp.GetValue<string>("Username"), smtp.GetValue<string>("Password"));

        var fromMail = "contact@mail.com";
        if(smtp.GetValue<string>("FromMail") != null)
            fromMail = smtp.GetValue<string>("FromMail");
        
        var message = new MailMessage(fromMail, email, subject, htmlMessage);
        message.IsBodyHtml = true;
        
        await client.SendMailAsync(message);
    }

    public async Task SendTemplatedEmailAsync<TModel>(string email, string subject, string viewName, TModel model)
    {
        try
        {
            var htmlMessage = await _engine.CompileRenderAsync($"{viewName}.cshtml", model);
            await SendEmailAsync(email, subject, htmlMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[RazorLight ERROR] {ex.Message}");
            throw;
        }
    }
}