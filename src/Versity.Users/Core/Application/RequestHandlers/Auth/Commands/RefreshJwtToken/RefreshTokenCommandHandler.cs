using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.Exceptions;
using MediatR;

namespace Application.RequestHandlers.Auth.Commands.RefreshJwtToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthTokens>
{
    private readonly IVersityUsersRepository _versityUsersRepository;
    private readonly IVersityRefreshTokensRepository _refreshTokensRepository;
    private readonly IAuthTokenGeneratorService _authTokenGeneratorService;
    private readonly IRefreshTokenGeneratorService _refreshTokenGeneratorService;

    public RefreshTokenCommandHandler(
        IVersityRefreshTokensRepository refreshTokensRepository, 
        IVersityUsersRepository versityUsersRepository, 
        IAuthTokenGeneratorService authTokenGeneratorService, 
        IRefreshTokenGeneratorService refreshTokenGeneratorService)
    {
        _refreshTokensRepository = refreshTokensRepository;
        _versityUsersRepository = versityUsersRepository;
        _authTokenGeneratorService = authTokenGeneratorService;
        _refreshTokenGeneratorService = refreshTokenGeneratorService;
    }

    public async Task<AuthTokens> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenGeneratorService.ValidateTokenAsync(request.UserId, request.RefreshToken, cancellationToken);
        var versityUser = await _versityUsersRepository.GetUserByIdAsync(refreshToken.UserId);
        if (versityUser is null)
        {
            throw new NotFoundExceptionWithStatusCode("There is no user with this Id");
        }
        
        var userRoles = await _versityUsersRepository.GetRolesAsync(versityUser);
        var userToken = _authTokenGeneratorService.GenerateToken(versityUser.Id, versityUser.Email, userRoles);
        
        refreshToken.IsUsed = true;
        _refreshTokensRepository.Update(refreshToken);
        await _refreshTokensRepository.SaveChangesAsync(cancellationToken);

        return new AuthTokens(request.UserId, userToken, refreshToken.Token);
    }
}