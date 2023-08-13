using Infrastructure.Exceptions;
using Infrastructure.Services.TokenServices;

namespace Presentation.Configuration;

public class TokenGenerationConfiguration : ITokenGenerationConfiguration
{
    public string Issuer { get; } = Environment.GetEnvironmentVariable("JWT__Issuer") ?? "none";
    public string Audience { get; } = Environment.GetEnvironmentVariable("JWT__Audience") ?? "none";
    public string Key { get; } = Environment.GetEnvironmentVariable("JWT__Key") ?? "none";

    public TokenGenerationConfiguration(IConfiguration configuration)
    {
        Issuer = configuration["Jwt:Issuer"] ?? throw new UserSecretsInvalidException("setup-jwt-issuer-secret");
        Audience = configuration["Jwt:Audience"] ?? throw new UserSecretsInvalidException("setup-jwt-audience-secret");
        Key = configuration["Jwt:Key"] ?? throw new UserSecretsInvalidException("setup-jwt-key-secrets");
    }
}