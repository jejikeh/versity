using Application.Abstractions;
using Application.Abstractions.Repositories;

namespace Presentation.Authentication;

public class RefreshJwtTokenMiddleware : IMiddleware
{
    private readonly IVersityUsersRepository _versityUsersRepository;
    private readonly IVersityRefreshTokensRepository _refreshTokensRepository;
    private readonly IAuthTokenGeneratorService _authTokenGeneratorService;
    private readonly IRefreshTokenGeneratorService _refreshTokenGeneratorService;
    private readonly RequestDelegate _next;

    public RefreshJwtTokenMiddleware(IVersityUsersRepository versityUsersRepository, IVersityRefreshTokensRepository refreshTokensRepository, IAuthTokenGeneratorService authTokenGeneratorService, IRefreshTokenGeneratorService refreshTokenGeneratorService, RequestDelegate next)
    {
        _versityUsersRepository = versityUsersRepository;
        _refreshTokensRepository = refreshTokensRepository;
        _authTokenGeneratorService = authTokenGeneratorService;
        _refreshTokenGeneratorService = refreshTokenGeneratorService;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var decryptedJwtToken = _authTokenGeneratorService.DecryptJwtTokenFromHeader();
        
        
        await _next(context);
    }
}