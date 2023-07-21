using Infrastructure.Services.EmailServices;

namespace Presentation.Configuration;

public class EmailServicesConfiguration : IEmailServicesConfiguration
{
    public string SmtpServer { get; set; } = Environment.GetEnvironmentVariable("EMAIL__SmtpServer");
    public int Port { get; set; } = int.Parse(Environment.GetEnvironmentVariable("EMAIL__Port"));
    public string Username { get; set; } = Environment.GetEnvironmentVariable("EMAIL__Username");
    public string Password { get; set; } = Environment.GetEnvironmentVariable("EMAIL__Password");
    public string From { get; set; } = Environment.GetEnvironmentVariable("EMAIL__From");
    public string ConfirmUrl { get; set; } = Environment.GetEnvironmentVariable("EMAIL__ConfirmUrl");
}