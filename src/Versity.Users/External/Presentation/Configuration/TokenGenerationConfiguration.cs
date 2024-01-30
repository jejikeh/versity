using Infrastructure.Services.TokenServices;

namespace Presentation.Configuration;

public class TokenGenerationConfiguration : ITokenGenerationConfiguration
{
    public string Issuer { get; } = Environment.GetEnvironmentVariable("JWT__Issuer") ?? "none";
    public string Audience { get; } = Environment.GetEnvironmentVariable("JWT__Audience") ?? "none";
    public string Key { get; } = Environment.GetEnvironmentVariable("JWT__Key") ?? "none";
}