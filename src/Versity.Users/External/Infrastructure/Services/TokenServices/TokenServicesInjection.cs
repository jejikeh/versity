using Application.Abstractions;
using Infrastructure.Services.EmailServices;
using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.TokenServices;

public static class TokenServicesInjection
{
    public static IServiceCollection UseTokenServices(this IServiceCollection serviceCollection, ITokenGenerationConfiguration configuration)
    {
        serviceCollection.AddSingleton(configuration);
        serviceCollection.AddScoped<IAuthTokenGeneratorService, JwtTokenGeneratorService>();
        serviceCollection.AddScoped<IRefreshTokenGeneratorService, RefreshTokenGeneratorService>();
        
        return serviceCollection;
    }
}