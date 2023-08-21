using Domain.Models.SessionLogging;

namespace Application.Abstractions.Repositories;

public interface ISessionLogsRepository
{
    public IEnumerable<SessionLogs> GetSessionsLogs(int? skipCount, int? takeCount);
    public Task<SessionLogs?> GetSessionLogsByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<SessionLogs> CreateSessionLogsAsync(SessionLogs sessionLogs, CancellationToken cancellationToken);
    public Task CreateSessionLogsRangeAsync(ICollection<SessionLogs> sessionsLogs, CancellationToken cancellationToken);
    public Task SaveChangesAsync(CancellationToken cancellationToken);
    public void SaveChanges();
}