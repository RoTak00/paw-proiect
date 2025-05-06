using System.Net;
using System.Net.Mail;

namespace PAW_Project.Services;

public class SmtpEmailSender : IEmailService
{
    private readonly IConfiguration _configuration;

    public SmtpEmailSender(IConfiguration config)
    {
        _configuration = config;
    }

    public async Task SendEmailAsync(string email, string subject, string html_message)
    {
        var smtp = _configuration.GetSection("Smtp");

        var client = new SmtpClient(smtp.GetValue<string>("Host"), Convert.ToInt32(smtp.GetValue<int>("Port")));
        client.EnableSsl = Convert.ToBoolean(smtp.GetValue<bool>("EnableSsl"));
        client.Credentials = new NetworkCredential(smtp.GetValue<string>("Username"), smtp.GetValue<string>("Password"));
        
        var message = new MailMessage(smtp.GetValue<string>("FromMail"), email, subject, html_message);
        message.IsBodyHtml = true;
        
        await client.SendMailAsync(message);
    }
}