using System.Text;
using Application.Abstractions;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Services;
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

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection serviceCollection, IConfiguration configuration)
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
                ValidIssuer = Environment.GetEnvironmentVariable("JWT__Issuer") ?? configuration.GetSection("Jwt:Issuer").Value,
                ValidAudience = Environment.GetEnvironmentVariable("JWT__Audience") ?? configuration.GetSection("Jwt:Audience").Value,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT__Key") ?? configuration.GetSection("Jwt:Key").Value))
            };
        });

        return serviceCollection;
    }
}