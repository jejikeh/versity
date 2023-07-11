using Application.Abstractions.Repositories;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Persistence.Repositories;

public class CachedSessionsRepository : ICachedSessionsRepository
{
    private readonly SessionsRepository _sessions;
    private readonly IMemoryCache _memoryCache;

    public CachedSessionsRepository(SessionsRepository sessions, IMemoryCache memoryCache)
    {
        _sessions = sessions;
        _memoryCache = memoryCache;
    }

    public IQueryable<Session> GetAllSessions()
    {
        return _sessions.GetAllSessions();
    }

    public Task<Session?> GetSessionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _memoryCache.GetOrCreateAsync(
            $"session-by-id-{id}",
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                
                return _sessions.GetSessionByIdAsync(id, cancellationToken);
            });
    }

    public IQueryable<Session> GetAllUserSessions(string userId, CancellationToken cancellationToken)
    {
        return _sessions.GetAllUserSessions(userId, cancellationToken);
    }

    public IQueryable<Session> GetAllProductSessions(Guid productId, CancellationToken cancellationToken)
    {
        return _sessions.GetAllProductSessions(productId, cancellationToken);
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