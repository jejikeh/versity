namespace Infrastructure.Services.TokenServices;

public interface ITokenGenerationConfiguration
{
    public string Issuer { get; }
    public string Audience { get; }
    public string Key { get; }
}