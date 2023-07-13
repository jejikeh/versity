using Application.Abstractions.Repositories;
using Domain.Models;
using Microsoft.Extensions.Caching.Distributed;
using Infrastructure.Extensions;

namespace Infrastructure.Persistence.Repositories;

public class CachedRefreshTokensRepository : IVersityRefreshTokensRepository
{
    private readonly IVersityRefreshTokensRepository _refreshTokensRepository;
    private readonly IDistributedCache _distributedCache;
    private readonly VersityUsersDbContext _context;

    public CachedRefreshTokensRepository(IVersityRefreshTokensRepository refreshTokensRepository, IDistributedCache distributedCache, VersityUsersDbContext context)
    {
        _refreshTokensRepository = refreshTokensRepository;
        _distributedCache = distributedCache;
        _context = context;
    }

    public async Task<IEnumerable<RefreshToken>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _distributedCache.GetOrCreateAsync(
            _context,
            "tokens",
            cancellationToken,
            async () => await _refreshTokensRepository.GetAllAsync(cancellationToken));
    }

    public Task AddAsync(RefreshToken token, CancellationToken cancellationToken)
    {
        return _refreshTokensRepository.AddAsync(token, cancellationToken);
    }

    public RefreshToken Update(RefreshToken token)
    {
        return _refreshTokensRepository.Update(token);
    }

    public async Task<RefreshToken> FindUserTokenAsync(string userId, string token, CancellationToken cancellationToken)
    {
        return await _distributedCache.GetOrCreateAsync(
            _context,
            $"token-{token}-user-{userId}",
            cancellationToken,
            async () => await _refreshTokensRepository.FindUserTokenAsync(userId, token, cancellationToken));
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _refreshTokensRepository.SaveChangesAsync(cancellationToken);
    }
}