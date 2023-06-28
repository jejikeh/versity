using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.Exceptions;
using Domain.Models;
using MediatR;

namespace Application.RequestHandlers.Auth.Commands.LoginVersityUser;

public class LoginVersityUserCommandHandler : IRequestHandler<LoginVersityUserCommand, AuthTokens>
{
    private readonly IVersityUsersRepository _versityUsersRepository;
    private readonly IVersityRefreshTokensRepository _refreshTokensRepository;
    private readonly IAuthTokenGeneratorService _authTokenGeneratorService;
    private readonly IRefreshTokenGeneratorService _refreshTokenGeneratorService;

    public LoginVersityUserCommandHandler(
        IAuthTokenGeneratorService authTokenGeneratorService, 
        IVersityUsersRepository versityUsersRepository, 
        IVersityRefreshTokensRepository refreshTokensRepository, 
        IRefreshTokenGeneratorService refreshTokenGeneratorService)
    {
        _authTokenGeneratorService = authTokenGeneratorService;
        _versityUsersRepository = versityUsersRepository;
        _refreshTokensRepository = refreshTokensRepository;
        _refreshTokenGeneratorService = refreshTokenGeneratorService;
    }

    public async Task<AuthTokens> Handle(LoginVersityUserCommand request, CancellationToken cancellationToken)
    {
        var versityUser = await GetUserByEmail(request);
        var userRoles = await _versityUsersRepository.GetRolesAsync(versityUser);
        var userToken = _authTokenGeneratorService.GenerateToken(versityUser.Id, versityUser.Email, userRoles);
        var refreshToken = _refreshTokenGeneratorService.GenerateToken(versityUser.Id);
        await _refreshTokensRepository.AddAsync(refreshToken, cancellationToken);
        await _refreshTokensRepository.SaveChangesAsync(cancellationToken);

        return new AuthTokens(versityUser.Id, userToken, refreshToken.Token);
    }

    private async Task<VersityUser?> GetUserByEmail(LoginVersityUserCommand request)
    {
        var versityUser = await _versityUsersRepository.GetUserByEmailAsync(request.Email);
        if (versityUser is null || !await _versityUsersRepository.CheckPasswordAsync(versityUser, request.Password))
        {
            throw new IncorrectEmailOrPasswordExceptionWithStatusCode();
        }
        if (!versityUser.EmailConfirmed)
        {
            throw new EmailNotConfirmedExceptionWithStatusCode();
        }

        return versityUser;
    }
}