using Application.Abstractions.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Infrastructure.Persistence.SqlRepositories;

public class SessionsSqlRepository : ISessionsRepository
{
    private readonly VersitySessionsServiceSqlDbContext _context;

    public SessionsSqlRepository(VersitySessionsServiceSqlDbContext context)
    {
        _context = context;
    }


    public IEnumerable<Session> GetSessions(int? skipCount, int? takeCount)
    {
        if (skipCount is null && takeCount is null)
        {
            return _context.Sessions
                .AsQueryable()
                .OrderBy(data => data.Start)
                .ToList();
        }
        
        return _context.Sessions
            .AsQueryable()
            .OrderBy(data => data.Start)
            .Skip(skipCount ?? 0)
            .Take(takeCount ?? 10)
            .ToList();
    }

    public IEnumerable<Session> GetExpiredSessions()
    {
        return _context.Sessions
            .AsQueryable()
            .Where(x => x.Expiry < DateTime.UtcNow)
            .Where(x => x.Status != SessionStatus.Closed && x.Status != SessionStatus.Expired)
            .ToList();
    }

    public IEnumerable<Session> GetInactiveSessions()
    {
        return _context.Sessions
            .AsQueryable()
            .Where(x => x.Start <= DateTime.UtcNow && x.Expiry > DateTime.UtcNow)
            .Where(x => x.Status == SessionStatus.Inactive)
            .ToList();
    }

    public async Task<Session?> GetSessionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Sessions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public IEnumerable<Session> GetAllUserSessions(string userId, int? skipCount, int? takeCount)
    {
        return _context.Sessions
            .AsQueryable()
            .Where(data => data.UserId == userId)
            .Skip(skipCount ?? 0)
            .Take(takeCount ?? 10)
            .ToList();
    }

    public IEnumerable<Session> GetAllProductSessions(Guid productId, int? skipCount, int? takeCount)
    {
        return _context.Sessions
            .AsQueryable()
            .Where(data => data.ProductId == productId)
            .Skip(skipCount ?? 0)
            .Take(takeCount ?? 10)
            .ToList();
    }

    public async Task<Session> CreateSessionAsync(Session session, CancellationToken cancellationToken)
    {
        var entityEntry = await _context.AddAsync(session, cancellationToken);

        return entityEntry.Entity;
    }

    public async Task CreateSessionRangeAsync(ICollection<Session> sessions, CancellationToken cancellationToken)
    {
        await _context.AddRangeAsync(sessions, cancellationToken);
    }

    public Task<Session> UpdateSessionAsync(Session session, CancellationToken cancellationToken)
    {
        return Task.FromResult(_context.Update(session).Entity);
    }

    public Session UpdateSession(Session session)
    {
        return _context.Update(session).Entity;
    }

    public void DeleteSession(Session session)
    {
        _context.Remove(session);
    }

    public Task<List<Session>> ToListAsync(IQueryable<Session> sessions)
    {
        return sessions.ToListAsync();
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}