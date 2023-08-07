using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Common;
using Domain.Models;

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
        return _sessions.GetAllSessions();
    }

    public async Task<Session?> GetSessionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _distributedCache.GetOrCreateAsync(
            CachingKeys.SessionById(id),
            async () => await _sessions.GetSessionByIdAsync(id, cancellationToken));
    }

    public IQueryable<Session> GetAllUserSessions(string userId)
    {
        return _sessions.GetAllUserSessions(userId);
    }

    public IQueryable<Session> GetAllProductSessions(Guid productId)
    {
        return _sessions.GetAllProductSessions(productId);
    }

    public Task<Session> CreateSessionAsync(Session session, CancellationToken cancellationToken)
    {
        return _sessions.CreateSessionAsync(session, cancellationToken);
    }

    public Task CreateSessionRangeAsync(ICollection<Session> sessions, CancellationToken cancellationToken)
    {
        return _sessions.CreateSessionRangeAsync(sessions, cancellationToken);
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
        return _sessions.ToListAsync(sessions);
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