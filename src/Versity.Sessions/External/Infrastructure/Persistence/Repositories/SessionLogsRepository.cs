using Application.Abstractions.Repositories;
using Domain.Models.SessionLogging;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class SessionLogsRepository : ISessionLogsRepository
{
    private readonly VersitySessionsServiceDbContext _context;

    public SessionLogsRepository(VersitySessionsServiceDbContext context)
    {
        _context = context;
    }

    public IQueryable<SessionLogs> GetAllSessionsLogs()
    {
        return _context.SessionLogs
            .Include(x => x.Logs)
            .AsQueryable();
    }

    public async Task<SessionLogs?> GetSessionLogsByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.SessionLogs
            .Include(x => x.Logs)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<SessionLogs> CreateSessionLogsAsync(SessionLogs sessionLogs, CancellationToken cancellationToken)
    {
        var entity = await _context.AddAsync(sessionLogs, cancellationToken);

        return entity.Entity;
    }

    public async Task CreateRangeSessionLogsAsync(IEnumerable<SessionLogs> sessionsLogs, CancellationToken cancellationToken)
    {
        await _context.AddRangeAsync(sessionsLogs, cancellationToken);
    }

    public async Task<List<SessionLogs>> ToListAsync(IQueryable<SessionLogs> sessionLogs)
    {
        return await sessionLogs.ToListAsync();
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