using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Versity.Users.Core.Application.Abstractions;
using Versity.Users.Core.Domain.Models;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Versity.Users.Services;

public class JwtTokenGeneratorService : IAuthTokenGeneratorService
{
    private readonly IConfiguration _config;

    public JwtTokenGeneratorService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(VersityUser user)
    {
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Email, user.NormalizedEmail),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id)
        };
        
        var securityToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            issuer: _config.GetSection("Jwt:Issuer").Value,
            audience: _config.GetSection("Jwt:Audience").Value,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Key").Value)), 
                SecurityAlgorithms.HmacSha512Signature));
        
        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}