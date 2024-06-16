

using System.Net.Mail;

public class EmailService : IEmailService
{
    private readonly string _fromAddress;
    private readonly string _appPassword;
    private readonly string _fromAddressName;
    private readonly string _toAddressName;
    private readonly string _smtpServer;

    public EmailService()
    {
        _fromAddress = EMailServiceOptions.fromAddress;
        _appPassword = EMailServiceOptions.appPassword;
        _fromAddressName = EMailServiceOptions.fromAddressName;
        _toAddressName = EMailServiceOptions.toAddressName;
        _smtpServer = EMailServiceOptions.smtpServer;
    }
    public async Task SendEmailAsync(string toAddress, string subject, string body, bool isHtml)
    {
        SmtpClient client = new SmtpClient(_smtpServer);

        client.UseDefaultCredentials = false;
        client.EnableSsl = true;

        System.Net.NetworkCredential basicAuthInfo = new System.Net.NetworkCredential(_fromAddress, _appPassword);
        client.Credentials = basicAuthInfo;

        MailAddress fromMailAddress = new MailAddress(_fromAddress, _fromAddressName);
        MailAddress toMailAddres = new MailAddress(toAddress, _toAddressName);
        MailAddress replyToAddress = new MailAddress(_fromAddress);

        MailMessage message = new MailMessage(fromMailAddress, toMailAddres);
        message.ReplyToList.Add(replyToAddress);

        message.Subject = subject;
        message.Body = body;


        message.SubjectEncoding = System.Text.Encoding.UTF8;
        message.BodyEncoding = System.Text.Encoding.UTF8;
        message.IsBodyHtml = isHtml;
        await client.SendMailAsync(message);
        client.Dispose();




    }
}