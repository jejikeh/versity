using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Sessions.IntegrationTests.Helpers;

public static class JwtTokenGeneratorService
{
    public static string GenerateToken(string userId, string userEmail, IEnumerable<string> roles)
    {
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, userEmail),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture))
        };
        
        claims.AddRange(roles.Select(userRole => new Claim("role", userRole)));

        var securityToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(6),
            issuer: "versity.identity",
            audience: "versity.identity",
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("865D92FD-B1C8-41A4-850F-409792C9B740")), 
                SecurityAlgorithms.HmacSha512Signature));
        
        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}