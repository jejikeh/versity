using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Common;
using Domain.Models;

namespace Infrastructure.Persistence.Repositories;

public class CachedRefreshTokensRepository : IVersityRefreshTokensRepository
{
    private readonly IVersityRefreshTokensRepository _refreshTokensRepository;
    private readonly ICacheService _distributedCache;

    public CachedRefreshTokensRepository(IVersityRefreshTokensRepository refreshTokensRepository, ICacheService distributedCache)
    {
        _refreshTokensRepository = refreshTokensRepository;
        _distributedCache = distributedCache;
    }

    public async Task<IEnumerable<RefreshToken>> GetAllAsync(
        int? skipEntitiesCount, 
        int? takeEntitiesCount,
        CancellationToken cancellationToken)
    {
        return await _distributedCache.GetOrCreateAsync(
            CachingKeys.Tokens,
            async () => await _refreshTokensRepository.GetAllAsync(skipEntitiesCount, takeEntitiesCount, cancellationToken)) ?? 
               await _refreshTokensRepository.GetAllAsync(skipEntitiesCount, takeEntitiesCount, cancellationToken);
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
            CachingKeys.UserToken(token, userId),
            async () => await _refreshTokensRepository.FindUserTokenAsync(userId, token, cancellationToken)) ?? 
               await _refreshTokensRepository.FindUserTokenAsync(userId, token, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _refreshTokensRepository.SaveChangesAsync(cancellationToken);
    }
}