using Application.Abstractions.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class VersityRefreshTokensRepository : IVersityRefreshTokensRepository
{
    private readonly VersityUsersDbContext _usersDbContext;

    public VersityRefreshTokensRepository(VersityUsersDbContext usersDbContext)
    {
        _usersDbContext = usersDbContext;
    }

    public async Task<IEnumerable<RefreshToken>> GetAllAsync(
        int? skipEntitiesCount, 
        int? takeEntitiesCount,
        CancellationToken cancellationToken)
    {
        if (skipEntitiesCount is null && takeEntitiesCount is null)
        {
            return await _usersDbContext.RefreshTokens
                .AsQueryable()
                .OrderBy(refreshToken => refreshToken.ExpiryTime)
                .ToListAsync(cancellationToken);
        }
        
        return await _usersDbContext.RefreshTokens
            .AsQueryable()
            .OrderBy(refreshToken => refreshToken.ExpiryTime)
            .Skip(skipEntitiesCount ?? 0)
            .Take(takeEntitiesCount ?? 10)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken)
    {
        await _usersDbContext.RefreshTokens.AddAsync(token, cancellationToken);
    }

    public RefreshToken Update(RefreshToken token)
    {
        return _usersDbContext.RefreshTokens.Update(token).Entity;
    }

    public async Task<RefreshToken> FindUserTokenAsync(string userId, string token, CancellationToken cancellationToken)
    {
        return await _usersDbContext.RefreshTokens.SingleAsync(x => x.UserId == userId && x.Token == token, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _usersDbContext.SaveChangesAsync(cancellationToken);
    }
}