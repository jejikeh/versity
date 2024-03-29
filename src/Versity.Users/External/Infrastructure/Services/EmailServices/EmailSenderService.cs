﻿using Application.Abstractions;
using Application.Abstractions.Repositories;
using MailKit.Net.Smtp;
using MimeKit;

namespace Infrastructure.Services.EmailServices;

public class EmailSenderService : IEmailSenderService, IDisposable
{
    private readonly ISmtpClientService _smtpClient;
    private readonly IEmailServicesConfiguration _configuration;

    public EmailSenderService(ISmtpClientService smtpClient, IEmailServicesConfiguration configuration)
    {
        _smtpClient = smtpClient;
        _configuration = configuration;
    }
    
    public async Task SendEmailAsync(string name, string email, string subject, string message)
    {
        if (!_smtpClient.IsConnected)
        {
            _smtpClient.Connect();
        }
        
        await _smtpClient.SendAsync(CreateMimeMessage(name, message, subject, email));
    }
    
    private MimeMessage CreateMimeMessage(string name, string body, string subject, string email)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            name, 
            _configuration.From));
        
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