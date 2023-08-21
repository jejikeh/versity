using Domain.Models;

namespace Application.Abstractions.Repositories;

public interface ISessionsRepository
{
    public IEnumerable<Session> GetSessions(int? skipCount, int? takeCount);
    public IEnumerable<Session> GetExpiredSessions();
    public IEnumerable<Session> GetInactiveSessions();
    public Task<Session?> GetSessionByIdAsync(Guid id, CancellationToken cancellationToken);
    public IEnumerable<Session> GetAllUserSessions(string userId, int? skipCount, int? takeCount);
    public IEnumerable<Session> GetAllProductSessions(Guid productId, int? skipCount, int? takeCount);
    public Task<Session> CreateSessionAsync(Session session, CancellationToken cancellationToken);
    public Task CreateSessionRangeAsync(ICollection<Session> sessions, CancellationToken cancellationToken);
    public Task<Session> UpdateSessionAsync(Session session, CancellationToken cancellationToken);
    public Session UpdateSession(Session session);
    public void DeleteSession(Session session);
    public Task SaveChangesAsync(CancellationToken cancellationToken);
    public void SaveChanges();
}