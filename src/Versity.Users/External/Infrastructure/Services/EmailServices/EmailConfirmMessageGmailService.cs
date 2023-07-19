using System.Text;
using Application.Abstractions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Infrastructure.Services.EmailServices;

public class EmailConfirmMessageGmailService : IEmailConfirmMessageService
{
    private readonly UserManager<VersityUser> _userManager;
    private readonly IEmailSenderService _emailSenderService;

    public EmailConfirmMessageGmailService(UserManager<VersityUser> userManager, IEmailSenderService emailSenderService)
    {
        _userManager = userManager;
        _emailSenderService = emailSenderService;
    }

    public async Task SendEmailConfirmMessageAsync(VersityUser user)
    {
        var token = await GenerateConfirmationToken(user);
        
        var confirmUrl = Environment.GetEnvironmentVariable("EMAIL__ConfirmUrl") + $"{user.Id}/{token}";
        var emailBody = GenerateEmailBody(user, confirmUrl);

        await _emailSenderService.SendEmailAsync("Versity Identity Server", user.Email, "Confirm Email", emailBody);
    }

    private static string GenerateEmailBody(VersityUser user, string confirmUrl)
    {
        return $"<h1>Привет {user.FirstName}! Ты мое солнышко :3</h1></br>" +
               $"Please confirm your email address <a href={System.Text.Encodings.Web.HtmlEncoder.Default.Encode(confirmUrl)}>Confirm</a>";
    }

    private async Task<string> GenerateConfirmationToken(VersityUser user)
    {
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var tokenGeneratedBytes = Encoding.UTF8.GetBytes(code);
        var codeEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);

        return codeEncoded;
    }
}