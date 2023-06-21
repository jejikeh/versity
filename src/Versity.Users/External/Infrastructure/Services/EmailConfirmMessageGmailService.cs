using System.ComponentModel.DataAnnotations;
using System.Xml.Schema;
using Application.Abstractions;
using Application.Exceptions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Authenticators;

namespace Infrastructure.Services;

public class EmailConfirmMessageMailgunService : IEmailConfirmMessageService
{
    private readonly UserManager<VersityUser> _userManager;
    private readonly IConfiguration _config;
    private readonly ILogger<EmailConfirmMessageMailgunService> _logger;

    public EmailConfirmMessageMailgunService(UserManager<VersityUser> userManager, IConfiguration config, ILogger<EmailConfirmMessageMailgunService> logger)
    {
        _userManager = userManager;
        _config = config;
        _logger = logger;
    }

    public async Task GenerateEmailConfirmMessageAsync(VersityUser user)
    {
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmUrl = $"https://localhost:8001/api/auth/confirmemail/{user.Id}/{code}";
        var emailBody = $"Please confirm your email address <a href={System.Text.Encodings.Web.HtmlEncoder.Default.Encode(confirmUrl)}>Confirm</a>";
        var result = SendEmail(emailBody, user.Email);

        if (!result.IsSuccessful)
        {
            _logger.LogError($"Error occured while sending API Request to Mailgun service:\n Error Message: {result.ErrorMessage}");
            throw new BadRequestExceptionWithStatusCode(result.ErrorMessage!);
        }
    }

    private RestResponse SendEmail(string body, string email)
    {
        var options = new RestClientOptions("https://api.mailgun.net/v3")
        {
            Authenticator = new HttpBasicAuthenticator("api",
                Environment.GetEnvironmentVariable("Mailgun__Private__ApiKey") ??
                _config.GetSection("Mailgun:PrivateApiKey").Value)
        };
        var client = new RestClient(options);
        var request = new RestRequest();
        request.AddParameter("domain", "sandbox584dcdad9f9c441c8068dd796b88f7c8.mailgun.org", ParameterType.UrlSegment);
        request.Resource = "{domain}/messages";
        request.AddParameter("from", "Versity Identity Server Sandbox <mailgun@sandbox584dcdad9f9c441c8068dd796b88f7c8.mailgun.org>");
        request.AddParameter("to", email);
        request.AddParameter("subject", "Email Verification ");
        request.AddParameter("text", body);
        request.Method = Method.Post;
        
        return client.Execute(request);
    }
}