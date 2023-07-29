using MailKit.Net.Smtp;

namespace Infrastructure.Services.EmailServices;

public sealed class SmptClientService : SmtpClient
{
    public SmptClientService(IEmailServicesConfiguration configuration)
    {
        Connect(
            configuration.SmtpServer, 
            configuration.Port, 
            true);

        AuthenticationMechanisms.Remove("XOAUTH2");
        
        Authenticate(
            configuration.Username,
            configuration.Password);
    }
}