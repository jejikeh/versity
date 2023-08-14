using System.Text;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Services.TokenServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Presentation.Extensions;

public static class IdentityAuthenticationProgramExtension
{
    public static IServiceCollection AddVersityIdentity(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddIdentity<VersityUser, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            options.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<VersityUsersDbContext>()
        .AddDefaultTokenProviders()
        .AddRoles<IdentityRole>();

        return serviceCollection;
    }

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection serviceCollection, 
        ITokenGenerationConfiguration tokenGenerationConfiguration)
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