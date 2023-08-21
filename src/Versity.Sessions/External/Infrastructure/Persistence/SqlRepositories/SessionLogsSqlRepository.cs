using Application.Abstractions.Repositories;
using Domain.Models.SessionLogging;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.SqlRepositories;

public class SessionLogsSqlRepository : ISessionLogsRepository
{
    private readonly VersitySessionsServiceSqlDbContext _context;

    public SessionLogsSqlRepository(VersitySessionsServiceSqlDbContext context)
    {
        _context = context;
    }

    public IEnumerable<SessionLogs> GetSessionsLogs(int? skipCount, int? takeCount)
    {
        if (skipCount is null && takeCount is null)
        {
            return _context.SessionLogs
                .AsQueryable()
                .OrderBy(data => data.Id)
                .ToList();
        }
        
        return _context.SessionLogs
            .AsQueryable()
            .OrderBy(data => data.Id)
            .Skip(skipCount ?? 0)
            .Take(takeCount ?? 10)
            .ToList();
    }

    public async Task<SessionLogs?> GetSessionLogsByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.SessionLogs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<SessionLogs> CreateSessionLogsAsync(SessionLogs sessionLogs, CancellationToken cancellationToken)
    {
        var entity = await _context.AddAsync(sessionLogs, cancellationToken);

        return entity.Entity;
    }

    public async Task CreateSessionLogsRangeAsync(ICollection<SessionLogs> sessionsLogs, CancellationToken cancellationToken)
    {
        await _context.AddRangeAsync(sessionsLogs, cancellationToken);
    }

    public async Task CreateRangeSessionLogsAsync(IEnumerable<SessionLogs> sessionsLogs, CancellationToken cancellationToken)
    {
        await _context.AddRangeAsync(sessionsLogs, cancellationToken);
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