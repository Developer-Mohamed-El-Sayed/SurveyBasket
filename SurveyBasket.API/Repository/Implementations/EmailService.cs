namespace SurveyBasket.API.Repository.Implementations;

public class EmailService(IOptions<MailSettings> options) : IEmailSender
{
    private readonly MailSettings _options = options.Value;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Don't worry all that it exist at documention 
        var message = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_options.Mail),
            Subject = subject,
        };
        message.To.Add(MailboxAddress.Parse(email));
        // generate email body 
        var builder = new BodyBuilder
        {
            HtmlBody = htmlMessage
        };
        message.Body = builder.ToMessageBody();
        var smtp = new SmtpClient();
        smtp.Connect(_options.Host, _options.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_options.Mail, _options.Password);
        await smtp.SendAsync(message);
        smtp.Disconnect(true);
    }
}
