

using System.Net.Mail;
public interface IEmailService
{
    public Task SendEmail(string toAddress, string subject, string body, bool isHtml);
}