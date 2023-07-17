using Application.Abstractions.Repositories;
using Domain.Models;
using Infrastructure.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Persistence.Repositories;

public class CachedSessionsRepository : ISessionsRepository
{
    private readonly ISessionsRepository _sessions;
    private readonly IDistributedCache _distributedCache;
    private readonly VersitySessionsServiceDbContext _context;
    
    public CachedSessionsRepository(ISessionsRepository sessions, IDistributedCache distributedCache, VersitySessionsServiceDbContext context)
    {
        _sessions = sessions;
        _distributedCache = distributedCache;
        _context = context;
    }

    public IQueryable<Session> GetAllSessions()
    {
        return _sessions.GetAllSessions();
    }

    public async Task<Session?> GetSessionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _distributedCache.GetOrCreateAsync(
            _context,
            $"session-by-id-{id}",
            cancellationToken,
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