using Domain.Models.SessionLogging;

namespace Application.Abstractions.Repositories;

public interface ISessionLogsRepository
{
    public IQueryable<SessionLogs> GetAllSessionsLogs();
    public Task<SessionLogs?> GetSessionLogsByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<SessionLogs> CreateSessionLogsAsync(SessionLogs sessionLogs, CancellationToken cancellationToken);
    public Task CreateSessionLogsRangeAsync(ICollection<SessionLogs> sessionsLogs, CancellationToken cancellationToken);
    public Task<List<SessionLogs>> ToListAsync(IQueryable<SessionLogs> sessionLogs);
    public Task SaveChangesAsync(CancellationToken cancellationToken);
    public void SaveChanges();
}