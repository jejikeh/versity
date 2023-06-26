using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Common;
using Application.Dtos;
using Application.Exceptions;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Application.RequestHandlers.Auth.Commands.RefreshRefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthTokens>
{
    private readonly IVersityUsersRepository _versityUsersRepository;
    private readonly IVersityRefreshTokensRepository _refreshTokensRepository;
    private readonly IAuthTokenGeneratorService _tokenGeneratorService;

    public RefreshTokenCommandHandler(IVersityRefreshTokensRepository refreshTokensRepository, IVersityUsersRepository versityUsersRepository, IAuthTokenGeneratorService tokenGeneratorService)
    {
        _refreshTokensRepository = refreshTokensRepository;
        _versityUsersRepository = versityUsersRepository;
        _tokenGeneratorService = tokenGeneratorService;
    }

    public async Task<AuthTokens> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var oldRefreshToken = await ValidateRefreshToken(request, cancellationToken);
        var versityUser = await _versityUsersRepository.GetUserByIdAsync(request.UserId);
        var userRoles = await _versityUsersRepository.GetRolesAsync(versityUser);
        var userToken = _tokenGeneratorService.GenerateToken(versityUser.Id, versityUser.Email, userRoles);
        var refreshToken = Utils.GenerateRefreshTokenForUserById(versityUser.Id);
        await _refreshTokensRepository.AddAsync(refreshToken, cancellationToken);
        await _refreshTokensRepository.SaveChangesAsync(cancellationToken);
        oldRefreshToken.IsUsed = true;
        _refreshTokensRepository.Update(oldRefreshToken);

        return new AuthTokens(userToken, refreshToken.Token);
    }

    private async Task<RefreshToken> ValidateRefreshToken(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokens = await _refreshTokensRepository.GetAllUserTokensByUserIdAsync(request.UserId, cancellationToken);
        var refreshToken = tokens.FirstOrDefault(x => x.Token == request.RefreshToken);
        if (refreshToken is null)
        {
            throw new IdentityExceptionWithStatusCode("The refresh token was not generated.");
        }
        if (!refreshToken.IsRevoked && refreshToken.ExpiryTime > DateTime.UtcNow)
        {
            throw new IdentityExceptionWithStatusCode("The refresh token was expired or revoked. Please login again");
        }

        return refreshToken;
    }
}