using Application.Abstractions.Repositories;
using Domain.Models.SessionLogging;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Infrastructure.Persistence.MongoRepositories;

public class SessionLogsMongoRepository : ISessionLogsRepository
{
    private readonly VersitySessionsServiceMongoDbContext _context;

    public SessionLogsMongoRepository(VersitySessionsServiceMongoDbContext context)
    {
        _context = context;
    }

    public IEnumerable<SessionLogs> GetSessionsLogs(int? skipCount, int? takeCount)
    {
        if (skipCount is null && takeCount is null)
        {
            return _context.SessionLogs
                .AsQueryable()
                .OrderBy(data => data.Id);
        }

        return _context.SessionLogs
            .AsQueryable()
            .OrderBy(data => data.Id)
            .Skip(skipCount ?? 0)
            .Take(takeCount ?? 10);
    }

    public async Task<SessionLogs?> GetSessionLogsByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _context.SessionLogs.FindAsync(x => x.Id == id, cancellationToken: cancellationToken);

        return await result.FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task<SessionLogs> CreateSessionLogsAsync(SessionLogs sessionLogs, CancellationToken cancellationToken)
    {
        await _context.SessionLogs.InsertOneAsync(sessionLogs, cancellationToken: cancellationToken);

        return sessionLogs;
    }

    public async Task CreateSessionLogsRangeAsync(ICollection<SessionLogs> sessionsLogs, CancellationToken cancellationToken)
    {
        await _context.SessionLogs.InsertManyAsync(sessionsLogs, cancellationToken: cancellationToken);
    }

    public async Task<List<SessionLogs>> ToListAsync(IQueryable<SessionLogs> sessionLogs)
    {
        return await sessionLogs.ToListAsync();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void SaveChanges()
    {
    }
}