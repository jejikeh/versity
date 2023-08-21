using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Versity.ApiGateway.Configuration;

namespace Versity.ApiGateway.Extensions;

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
        }).AddJwtBearer(options =>
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
        });

        return serviceCollection;
    }
}