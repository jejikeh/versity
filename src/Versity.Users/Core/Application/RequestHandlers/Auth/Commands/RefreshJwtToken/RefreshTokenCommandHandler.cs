using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Dtos;
using MediatR;

namespace Application.RequestHandlers.Auth.Commands.RefreshJwtToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthTokens>
{
    private readonly IVersityUsersRepository _versityUsersRepository;
    private readonly IVersityRefreshTokensRepository _refreshTokensRepository;
    private readonly IAuthTokenGeneratorService _authTokenGeneratorService;
    private readonly IRefreshTokenGeneratorService _refreshTokenGeneratorService;

    public RefreshTokenCommandHandler(IVersityRefreshTokensRepository refreshTokensRepository, IVersityUsersRepository versityUsersRepository, IAuthTokenGeneratorService authTokenGeneratorService, IRefreshTokenGeneratorService refreshTokenGeneratorService)
    {
        _refreshTokensRepository = refreshTokensRepository;
        _versityUsersRepository = versityUsersRepository;
        _authTokenGeneratorService = authTokenGeneratorService;
        _refreshTokenGeneratorService = refreshTokenGeneratorService;
    }

    public async Task<AuthTokens> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var decryptedJwtToken = _authTokenGeneratorService.DecryptJwtTokenFromHeader();
        var userIdClaim = _authTokenGeneratorService.GetUserIdClaimFromJwtToken(decryptedJwtToken);
        var refreshToken = await _refreshTokenGeneratorService.ValidateTokenAsync(userIdClaim.Value, request.RefreshToken, cancellationToken);
        var versityUser = await _versityUsersRepository.GetUserByIdAsync(userIdClaim.Value);
        var userRoles = await _versityUsersRepository.GetRolesAsync(versityUser);
        var userToken = _authTokenGeneratorService.GenerateToken(versityUser.Id, versityUser.Email, userRoles);
        refreshToken.IsUsed = true;
        _refreshTokensRepository.Update(refreshToken);
        await _refreshTokensRepository.SaveChangesAsync(cancellationToken);

        return new AuthTokens(userToken, refreshToken.Token);
    }
}