using Infrastructure.Services.EmailServices;

namespace Presentation.Configuration;

public class EmailServicesConfiguration : IEmailServicesConfiguration
{
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string From { get; set; }
    public string ConfirmUrl { get; set; }

    public EmailServicesConfiguration(IConfiguration configuration)
    {
        SmtpServer = configuration.GetConfigurationStringOrThrowException("Smtp:Server");
        Port = int.Parse(configuration.GetConfigurationStringOrThrowException("Smtp:Port"));
        Username = configuration.GetConfigurationStringOrThrowException("Smtp:Username");
        Password = configuration.GetConfigurationStringOrThrowException("Smtp:Password");
        From = configuration.GetConfigurationStringOrThrowException("Smtp:From");
        ConfirmUrl = configuration.GetConfigurationStringOrThrowException("Smtp:ConfirmUrl");
    }
}

file static class ConfigurationExtension
{
    public static string GetConfigurationStringOrThrowException(
        this IConfiguration configuration,
        string configurationString)
    {
        return configuration[configurationString] 
               ?? throw new InvalidOperationException($"The {configurationString} is empty. Please set the {configurationString} in appsettings.json");
    }
}