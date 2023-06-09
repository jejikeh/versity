﻿using System.Text;
using Application.Abstractions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.WebUtilities;

namespace Infrastructure.Services;

public class EmailConfirmMessageGmailService : IEmailConfirmMessageService
{
    private readonly UserManager<VersityUser> _userManager;
    private readonly IConfiguration _config;

    public EmailConfirmMessageGmailService(UserManager<VersityUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task SendEmailConfirmMessageAsync(VersityUser user)
    {
        var token = await GenerateConfirmationToken(user);
        
        var confirmUrl = Environment.GetEnvironmentVariable("EMAIL__ConfirmUrl") + $"{user.Id}/{token}";
        var emailBody = $"<h1>Привет {user.FirstName}! Ты мое солнышко :3</h1></br>" +
                        $"Please confirm your email address <a href={System.Text.Encodings.Web.HtmlEncoder.Default.Encode(confirmUrl)}>Confirm</a>";
        
        var message = CreateEmailMessage(emailBody, user.Email);
        SendEmailMessage(message);
    }

    private async Task<string> GenerateConfirmationToken(VersityUser user)
    {
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var tokenGeneratedBytes = Encoding.UTF8.GetBytes(code);
        var codeEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);

        return codeEncoded;
    }

    private MimeMessage CreateEmailMessage(string body, string email)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            "Versity Identity Server", 
            Environment.GetEnvironmentVariable("EMAIL__From")));
        
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = "Confirm Email";
        message.Body = new TextPart("html")
        {
            Text = body
        };

        return message;
    }

    private void SendEmailMessage(MimeMessage message)
    {
        using var client = new SmtpClient();
        client.Connect(
            Environment.GetEnvironmentVariable("EMAIL__SmtpServer"), 
            int.Parse(Environment.GetEnvironmentVariable("EMAIL__Port")), 
            true);

        client.AuthenticationMechanisms.Remove("XOAUTH2");
        client.Authenticate(
            Environment.GetEnvironmentVariable("EMAIL__Username"), 
            Environment.GetEnvironmentVariable("EMAIL__Password"));

        client.Send(message);
        client.Disconnect(true);
    } 
}