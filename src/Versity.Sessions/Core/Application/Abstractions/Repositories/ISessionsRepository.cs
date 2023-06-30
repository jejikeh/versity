using Domain.Models;

namespace Application.Abstractions.Repositories;

public interface ISessionsRepository
{
    public IQueryable<Session> GetAllSessions();
    public Task<Session?> GetSessionByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<IEnumerable<Session>> GetAllUserSessionsAsync(string userId, CancellationToken cancellationToken);
    public Task<IEnumerable<Session>> GetAllProductSessionsAsync(Guid productId, CancellationToken cancellationToken);
    public Task<Session> CreateSessionAsync(Session session, CancellationToken cancellationToken);
    public Session UpdateSession(Session session);
    public void DeleteSession(Session session);
    public Task SaveChangesAsync(CancellationToken cancellationToken);
}