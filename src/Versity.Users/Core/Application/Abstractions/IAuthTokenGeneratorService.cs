using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Application.Abstractions;

public interface IAuthTokenGeneratorService
{
    public string GenerateToken(string userId, string userEmail, IEnumerable<string> roles);
    public JwtSecurityToken DecryptJwtTokenFromHeader();
    public Claim GetUserIdClaimFromJwtToken(JwtSecurityToken decryptedJwtToken);
}