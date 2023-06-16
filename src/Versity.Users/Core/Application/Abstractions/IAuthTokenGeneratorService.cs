namespace Application.Abstractions;

public interface IAuthTokenGeneratorService
{
    public string GenerateToken(string userId, string userEmail, IEnumerable<string> roles);
}