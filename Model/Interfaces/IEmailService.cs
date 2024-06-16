

using System.Net.Mail;
public interface IEmailService
{
    public Task SendEmailAsync(string toAddress, string subject, string body, bool isHtml);
}