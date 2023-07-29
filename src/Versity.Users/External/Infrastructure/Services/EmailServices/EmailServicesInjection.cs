using Application.Abstractions;
using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.EmailServices;

public static class EmailServicesInjection
{
    public static IServiceCollection UseEmailServices(this IServiceCollection serviceCollection, IEmailServicesConfiguration configuration)
    {
        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddTransient<IEmailConfirmMessageService, SendConfirmMessageEmailService>();
        serviceCollection.AddTransient<IEmailSenderService, EmailSenderService>();
        serviceCollection.AddTransient<ISmtpClient, SmptClientService>();
        
        return serviceCollection;
    }
}