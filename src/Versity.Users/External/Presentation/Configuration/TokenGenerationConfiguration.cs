using Infrastructure.Services.TokenServices;

namespace Presentation.Configuration;

public class TokenGenerationConfiguration : ITokenGenerationConfiguration
{
    public string Issuer { get; } = Environment.GetEnvironmentVariable("JWT__Issuer");
    public string Audience { get; } = Environment.GetEnvironmentVariable("JWT__Audience");
    public string Key { get; } = Environment.GetEnvironmentVariable("JWT__Key");
}