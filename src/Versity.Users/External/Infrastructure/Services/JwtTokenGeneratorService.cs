using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Abstractions;
using Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Infrastructure.Services;

public class JwtTokenGeneratorService : IAuthTokenGeneratorService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtTokenGeneratorService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
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
            expires: DateTime.UtcNow.AddMinutes(1),
            issuer: Environment.GetEnvironmentVariable("JWT__Issuer"),
            audience: Environment.GetEnvironmentVariable("JWT__Audience"),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT__Key"))), 
                SecurityAlgorithms.HmacSha512Signature));
        
        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    public JwtSecurityToken DecryptJwtTokenFromHeader()
    {
        var oldJwtToken = _httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Authorization].ToString();
        if (oldJwtToken is null)
        {
            throw new IdentityExceptionWithStatusCode("The Authorization token was not provided!");
        }
        var decryptedJwtToken = DecryptToken(oldJwtToken);
        
        return decryptedJwtToken;
    }
    
    public Claim GetUserIdClaimFromJwtToken(JwtSecurityToken decryptedJwtToken)
    {
        var userIdClaim = decryptedJwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
        if (userIdClaim is null || string.IsNullOrEmpty(userIdClaim.Value))
        {
            throw new IdentityExceptionWithStatusCode("The Authorization token was corrupted!");
        }

        return userIdClaim;
    }
    
    private JwtSecurityToken DecryptToken(string encryptedToken)
    {
        if (encryptedToken.StartsWith("bearer", StringComparison.InvariantCultureIgnoreCase))
        {
            encryptedToken = encryptedToken[7..];
        }
        var handler = new JwtSecurityTokenHandler();
        var decryptedJwtToken = handler.ReadJwtToken(encryptedToken);
        
        return decryptedJwtToken;
    }
}