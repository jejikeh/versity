using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Presentation.Configuration;

namespace Presentation.Extensions;

public static class IdentityAuthenticationProgramExtension
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SetTokenValidationParameters();
            options.SetSignalRAuthorization();
        });

        serviceCollection.AddSingleton<IUserIdProvider, SubBasedUserIdProvider>();
        
        return serviceCollection;
    }

    private static JwtBearerOptions SetTokenValidationParameters(this JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateActor = true,
            ValidateIssuer = true,
            ValidateAudience = false,
            RequireExpirationTime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT__Issuer"),
            ValidAudience = Environment.GetEnvironmentVariable("JWT__Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT__Key")))
        };
        
        return options;
    }

    private static JwtBearerOptions SetSignalRAuthorization(this JwtBearerOptions options)
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/sessions-hub/"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };

        return options;
    }
}