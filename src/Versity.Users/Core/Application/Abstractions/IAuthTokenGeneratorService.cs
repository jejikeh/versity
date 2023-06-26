using System.IdentityModel.Tokens.Jwt;

namespace Application.Abstractions;

public interface IAuthTokenGeneratorService
{
    public string GenerateToken(string userId, string userEmail, IEnumerable<string> roles);
    public JwtSecurityToken DecryptToken(string encryptedToken);
}