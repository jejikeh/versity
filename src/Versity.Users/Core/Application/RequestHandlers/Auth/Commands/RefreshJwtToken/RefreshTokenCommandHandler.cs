using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Application.RequestHandlers.Auth.Commands.RefreshJwtToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthTokens>
{
    private readonly IVersityUsersRepository _versityUsersRepository;
    private readonly IVersityRefreshTokensRepository _refreshTokensRepository;
    private readonly IAuthTokenGeneratorService _authTokenGeneratorService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRefreshTokenGeneratorService _refreshTokenGeneratorService;

    public RefreshTokenCommandHandler(IVersityRefreshTokensRepository refreshTokensRepository, IVersityUsersRepository versityUsersRepository, IAuthTokenGeneratorService authTokenGeneratorService, IHttpContextAccessor httpContextAccessor, IRefreshTokenGeneratorService refreshTokenGeneratorService)
    {
        _refreshTokensRepository = refreshTokensRepository;
        _versityUsersRepository = versityUsersRepository;
        _authTokenGeneratorService = authTokenGeneratorService;
        _httpContextAccessor = httpContextAccessor;
        _refreshTokenGeneratorService = refreshTokenGeneratorService;
    }

    public async Task<AuthTokens> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var oldJwtToken = _httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Authorization].ToString();
        if (oldJwtToken is null)
        {
            throw new IdentityExceptionWithStatusCode("The Authorization token was not provided!");
        }
        var decryptedJwtToken = _authTokenGeneratorService.DecryptToken(oldJwtToken);
        
        var userIdClaim = decryptedJwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
        if (userIdClaim is null || string.IsNullOrEmpty(userIdClaim.Value)) 
        { 
            throw new IdentityExceptionWithStatusCode("The Authorization token was corrupted!");
        }
        var oldRefreshToken = await _refreshTokenGeneratorService.ValidateTokenAsync(userIdClaim.Value, request.RefreshToken, cancellationToken);
        var versityUser = await _versityUsersRepository.GetUserByIdAsync(userIdClaim.Value);
        var userRoles = await _versityUsersRepository.GetRolesAsync(versityUser);
        var userToken = _authTokenGeneratorService.GenerateToken(versityUser.Id, versityUser.Email, userRoles);
        oldRefreshToken.IsUsed = true;
        _refreshTokensRepository.Update(oldRefreshToken);
        await _refreshTokensRepository.SaveChangesAsync(cancellationToken);

        return new AuthTokens(userToken, request.RefreshToken);
    }
}