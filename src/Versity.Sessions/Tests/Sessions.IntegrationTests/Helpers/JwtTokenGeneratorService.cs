using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Products.Tests.Integrations.Helpers;

public class JwtTokenGeneratorService
{
    public string GenerateToken(string userId, string userEmail, IEnumerable<string> roles)
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
            issuer: Environment.GetEnvironmentVariable("JWT__Issuer"),
            audience: Environment.GetEnvironmentVariable("JWT__Audience"),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT__Key"))), 
                SecurityAlgorithms.HmacSha512Signature));
        
        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}