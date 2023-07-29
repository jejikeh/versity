using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Abstractions;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Infrastructure.Services.TokenServices;

public class JwtTokenGeneratorService : IAuthTokenGeneratorService
{
    private readonly ITokenGenerationConfiguration _configuration;
    
    public JwtTokenGeneratorService(ITokenGenerationConfiguration configuration)
    {
        _configuration = configuration;
    }

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
            issuer: _configuration.Issuer,
            audience: _configuration.Audience,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Key)), 
                SecurityAlgorithms.HmacSha512Signature));
        
        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}