using Application.Abstractions;
using MailKit.Net.Smtp;
using MimeKit;

namespace Infrastructure.Services.EmailServices;

public class EmailSenderService : IEmailSenderService, IDisposable
{
    private readonly ISmtpClient _smtpClient;

    public EmailSenderService(ISmtpClient smtpClient)
    {
        _smtpClient = smtpClient;
    }
    
    public async Task SendEmailAsync(string name, string email, string subject, string message)
    {
        await _smtpClient.SendAsync(CreateMimeMessage(name, message, subject, email));
    }
    
    private static MimeMessage CreateMimeMessage(string name, string body, string subject, string email)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            name, 
            Environment.GetEnvironmentVariable("EMAIL__From")));
        
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;
        message.Body = new TextPart("html")
        {
            Text = body
        };

        return message;
    }

    public void Dispose()
    {
        _smtpClient.Disconnect(true);
        _smtpClient.Dispose();
    }
}