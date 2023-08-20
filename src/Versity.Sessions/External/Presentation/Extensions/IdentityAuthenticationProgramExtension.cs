using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Presentation.Configuration;

namespace Presentation.Extensions;

public static class IdentityAuthenticationProgramExtension
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection serviceCollection,
        TokenGenerationConfiguration tokenGenerationConfiguration)
    {
        serviceCollection.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => options
                .SetValidationTokenOptions(tokenGenerationConfiguration)
                .SetSignalRAuthorization());
        
        serviceCollection.AddSingleton<IUserIdProvider, SubBasedUserIdProvider>();
        
        return serviceCollection;
    }

    private static JwtBearerOptions SetValidationTokenOptions(this JwtBearerOptions options, TokenGenerationConfiguration tokenGenerationConfiguration)
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateActor = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            RequireExpirationTime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = tokenGenerationConfiguration.Issuer,
            ValidAudience = tokenGenerationConfiguration.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenGenerationConfiguration.Key))
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
                
                if (!string.IsNullOrEmpty(accessToken) && path.Value.Contains("sessions-hub"))
                {
                    context.Token = accessToken;
                }
                
                return Task.CompletedTask;
            }
        };

        return options;
    }
}