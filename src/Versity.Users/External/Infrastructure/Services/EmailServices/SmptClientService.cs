using Application.Abstractions.Repositories;
using MailKit.Net.Smtp;

namespace Infrastructure.Services.EmailServices;

public class SmptClientService : SmtpClient, ISmtpClientService
{
    private readonly IEmailServicesConfiguration _configuration;
    
    public SmptClientService(IEmailServicesConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Connect()
    {
        Connect(
            _configuration.SmtpServer, 
            _configuration.Port, 
            true);

        AuthenticationMechanisms.Remove("XOAUTH2");
        
        Authenticate(
            _configuration.Username,
            _configuration.Password);
    }
}