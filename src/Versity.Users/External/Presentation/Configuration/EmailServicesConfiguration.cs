using Infrastructure.Services.EmailServices;

namespace Presentation.Configuration;

public class EmailServicesConfiguration : IEmailServicesConfiguration
{
    public string SmtpServer { get; set; } = Environment.GetEnvironmentVariable("EMAIL__SmtpServer") ?? "none";
    public int Port { get; set; } = int.Parse(Environment.GetEnvironmentVariable("EMAIL__Port") ?? "0");
    public string Username { get; set; } = Environment.GetEnvironmentVariable("EMAIL__Username") ?? "none";
    public string Password { get; set; } = Environment.GetEnvironmentVariable("EMAIL__Password") ?? "none";
    public string From { get; set; } = Environment.GetEnvironmentVariable("EMAIL__From") ?? "none"; 
    public string ConfirmUrl { get; set; } = Environment.GetEnvironmentVariable("EMAIL__ConfirmUrl") ?? "none";
}