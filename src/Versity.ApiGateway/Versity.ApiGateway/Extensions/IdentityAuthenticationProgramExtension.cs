using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Versity.ApiGateway.Services;
using Versity.ApiGateway.Services.Abstractions;

namespace Versity.ApiGateway.Extensions;

public static class IdentityAuthenticationProgramExtension
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddTransient<IAuthTokenGeneratorService, JwtTokenGeneratorService>();

        serviceCollection.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateActor = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Environment.GetEnvironmentVariable("JWT__Issuer") ?? configuration.GetSection("Jwt:Issuer").Value,
                ValidAudience = Environment.GetEnvironmentVariable("JWT__Audience") ?? configuration.GetSection("Jwt:Audience").Value,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT__Key") ?? configuration.GetSection("Jwt:Key").Value))
            };
        });

        return serviceCollection;
    }
}