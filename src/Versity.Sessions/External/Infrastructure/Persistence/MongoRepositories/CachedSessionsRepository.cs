using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Common;
using Domain.Models;

namespace Infrastructure.Persistence.MongoRepositories;

public class CachedSessionsRepository : ISessionsRepository
{
    private readonly ISessionsRepository _sessions;
    private readonly ICacheService _distributedCache;
    
    public CachedSessionsRepository(ISessionsRepository sessions, ICacheService distributedCache)
    {
        _sessions = sessions;
        _distributedCache = distributedCache;
    }

    public IEnumerable<Session> GetSessions(int? skipCount, int? takeCount)
    {
        return _sessions.GetSessions(skipCount, takeCount);
    }

    public IEnumerable<Session> GetExpiredSessions()
    {
        return _sessions.GetExpiredSessions();
    }

    public IEnumerable<Session> GetInactiveSessions()
    {
        return _sessions.GetInactiveSessions();
    }

    public async Task<Session?> GetSessionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _distributedCache.GetOrCreateAsync(
            CachingKeys.SessionById(id),
            async () => await _sessions.GetSessionByIdAsync(id, cancellationToken));
    }

    public IEnumerable<Session> GetAllUserSessions(string userId, int? skipCount, int? takeCount)
    {
       return _sessions.GetAllUserSessions(userId, skipCount, takeCount);
    }

    public IEnumerable<Session> GetAllProductSessions(Guid productId, int? skipCount, int? takeCount)
    {
        return _sessions.GetAllProductSessions(productId, skipCount, takeCount);
    }

    public Task<Session> CreateSessionAsync(Session session, CancellationToken cancellationToken)
    {
        return _sessions.CreateSessionAsync(session, cancellationToken);
    }

    public Task CreateSessionRangeAsync(ICollection<Session> sessions, CancellationToken cancellationToken)
    {
        return _sessions.CreateSessionRangeAsync(sessions, cancellationToken);
    }

    public Task<Session> UpdateSessionAsync(Session session, CancellationToken cancellationToken)
    {
        return _sessions.UpdateSessionAsync(session, cancellationToken);
    }
    
    public Session UpdateSession(Session session)
    {
        return _sessions.UpdateSession(session);
    }

    public void DeleteSession(Session session)
    {
        _sessions.DeleteSession(session);
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