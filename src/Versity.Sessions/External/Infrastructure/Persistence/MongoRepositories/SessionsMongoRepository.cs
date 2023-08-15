using Application.Abstractions.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Infrastructure.Persistence.MongoRepositories;

public class SessionsMongoRepository : ISessionsRepository
{
    private readonly VersitySessionsServiceMongoDbContext _context;

    public SessionsMongoRepository(VersitySessionsServiceMongoDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Session> GetSessions(int? skipCount, int? takeCount)
    {
        if (skipCount is null && takeCount is null)
        {
            return _context.Sessions
                .AsQueryable()
                .OrderBy(data => data.Id);
        }

        return _context.Sessions
            .AsQueryable()
            .OrderBy(data => data.Id)
            .Skip(skipCount ?? 0)
            .Take(takeCount ?? 10);
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
        var result =  await _context.Sessions.FindAsync(x => x.Id == id, cancellationToken: cancellationToken);

        return await result.SingleAsync(cancellationToken);
    }

    public IEnumerable<Session> GetAllUserSessions(string userId, int? skipCount, int? takeCount)
    {
        var result = _context.Sessions.Find(x => x.UserId == userId);
        
        return result
            .ToList()
            .OrderBy(data => data.Id)
            .Skip(skipCount ?? 0)
            .Take(takeCount ?? 10);
    }

    public IEnumerable<Session> GetAllProductSessions(Guid productId, int? skipCount, int? takeCount)
    {
        var result = _context.Sessions.Find(x => x.ProductId == productId);
        
        return result
            .ToList()
            .OrderBy(data => data.Id)
            .Skip(skipCount ?? 0)
            .Take(takeCount ?? 10);
    }

    public IQueryable<Session> GetAllProductSessions(Guid productId)
    {
        var result = _context.Sessions.Find(x => x.ProductId == productId);

        return result.ToList().AsQueryable();
    }

    public async Task<Session> CreateSessionAsync(Session session, CancellationToken cancellationToken)
    {
        await _context.Sessions.InsertOneAsync(session, cancellationToken: cancellationToken);

        return session;
    }

    public async Task CreateSessionRangeAsync(ICollection<Session> sessions, CancellationToken cancellationToken)
    {
        await _context.Sessions.InsertManyAsync(sessions, cancellationToken: cancellationToken);
    }

    public async Task<Session> UpdateSessionAsync(Session session, CancellationToken cancellationToken)
    {
        var filterDefinition = Builders<Session>.Filter.Eq(sess => sess.Id, session.Id);
        await _context.Sessions.ReplaceOneAsync(filterDefinition, session, cancellationToken: cancellationToken);

        return await GetSessionByIdAsync(session.Id, cancellationToken) ?? session;
    }

    public Session UpdateSession(Session session)
    {
        var filterDefinition = Builders<Session>.Filter.Eq(sess => sess.Id, session.Id);
        _context.Sessions.ReplaceOne(filterDefinition, session);

        var result =  _context.Sessions.Find(filterDefinition);

        return result.Single();
    }

    public async void DeleteSession(Session session)
    {
        var filterDefinition = Builders<Session>.Filter.Eq(prod => prod.Id, session.Id);
        await _context.Sessions.DeleteOneAsync(filterDefinition);
    }

    public async Task<List<Session>> ToListAsync(IQueryable<Session> sessions)
    {
        return await sessions.ToListAsync();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void SaveChanges()
    {
    }
}