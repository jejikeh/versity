﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Infrastructure.Services;

public class JwtTokenGeneratorService : IAuthTokenGeneratorService
{
    private readonly IConfiguration _config;

    public JwtTokenGeneratorService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(string userId, string userEmail, params string[] roles)
    {
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, userEmail)
        };
        claims.AddRange(roles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

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