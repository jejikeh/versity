using Application.Abstractions;
using Application.Abstractions.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CachedSessionsRepository : ISessionsRepository
{
    private readonly ISessionsRepository _sessions;
    private readonly ICacheService _distributedCache;
    
    public CachedSessionsRepository(ISessionsRepository sessions, ICacheService distributedCache)
    {
        _sessions = sessions;
        _distributedCache = distributedCache;
    }

    public IQueryable<Session> GetAllSessions()
    {
        return _distributedCache.GetSetOrAddRangeQueryableAsync(
            "all-sessions", 
            _sessions.GetAllSessions);
    }

    public async Task<Session?> GetSessionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _distributedCache.GetOrCreateAsync(
            $"session-by-id-{id}",
            async () => await _sessions.GetSessionByIdAsync(id, cancellationToken));
    }

    public IQueryable<Session> GetAllUserSessions(string userId)
    {
        return _distributedCache.GetSetOrAddRangeQueryableAsync(
            "user-sessions", 
            _sessions.GetAllSessions);
    }

    public IQueryable<Session> GetAllProductSessions(Guid productId)
    {
        return _distributedCache.GetSetOrAddRangeQueryableAsync(
            "product-sessions", 
            _sessions.GetAllSessions);
    }

    public Task<Session> CreateSessionAsync(Session session, CancellationToken cancellationToken)
    {
        return _sessions.CreateSessionAsync(session, cancellationToken);
    }

    public Session UpdateSession(Session session)
    {
        return _sessions.UpdateSession(session);
    }

    public void DeleteSession(Session session)
    {
        _sessions.DeleteSession(session);
    }

    public Task<List<Session>> ToListAsync(IQueryable<Session> sessions)
    {
        return sessions is IAsyncEnumerable<Session> ? _sessions.ToListAsync(sessions) : Task.Run(sessions.ToList);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _sessions.SaveChangesAsync(cancellationToken);
    }

    public void SaveChanges()
    {
        _sessions.SaveChanges();
    }
}