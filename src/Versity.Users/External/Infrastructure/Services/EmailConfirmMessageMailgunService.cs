using System.ComponentModel.DataAnnotations;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using RestSharp;

namespace Infrastructure.Services;

public class EmailConfirmMessageService
{
    private readonly UserManager<VersityUser> _userManager;

    public EmailConfirmMessageService(UserManager<VersityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task GenerateEmailConfirmMessage(VersityUser user)
    {
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmUrl = $"https://localhost:4001/api/auth/verifyemail/userid={user.Id}&code={code}";
        var emailBody = $"Please confirm your email address <a href={System.Text.Encodings.Web.HtmlEncoder.Default.Encode(confirmUrl)}>Confirm</a>";
    }

    private void SendEmail(string body, string email)
    {
        var client = new RestClient("https://")
    }
}