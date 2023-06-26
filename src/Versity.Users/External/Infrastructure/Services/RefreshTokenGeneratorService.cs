using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Common;
using Application.Exceptions;
using Domain.Models;

namespace Infrastructure.Services;

public class RefreshTokenGeneratorService : IRefreshTokenGeneratorService
{
    private readonly IVersityRefreshTokensRepository _refreshTokensRepository;

    public RefreshTokenGeneratorService(IVersityRefreshTokensRepository refreshTokensRepository)
    {
        _refreshTokensRepository = refreshTokensRepository;
    }

    public RefreshToken GenerateToken(string userId)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = Utils.GenerateRandomString(7),
            IsUsed = false,
            IsRevoked = false,
            AddedTime = DateTime.UtcNow,
            ExpiryTime = DateTime.UtcNow.AddMinutes(20)
        };
    }

    public async Task<RefreshToken> ValidateTokenAsync(string userId, string token, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokensRepository.FindUserTokenAsync(userId, token, cancellationToken);
        if (refreshToken is null)
        {
            throw new IdentityExceptionWithStatusCode("The refresh token was not generated.");
        }
        if (refreshToken.IsRevoked || refreshToken.ExpiryTime < DateTime.UtcNow)
        {
            throw new IdentityExceptionWithStatusCode("The refresh token was expired or revoked. Please login again");
        }

        return refreshToken;
    }
}