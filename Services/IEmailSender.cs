namespace Centers.API.Services;
public interface IEmailSender
{
    Task SendEmailAsync(string recipientEmail, string subject, string htmlBody);
}
