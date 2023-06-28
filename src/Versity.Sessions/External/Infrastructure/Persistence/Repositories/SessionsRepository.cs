using Application.Abstractions.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class SessionsRepository : ISessionsRepository
{
    private readonly VersitySessionsServiceDbContext _context;

    public SessionsRepository(VersitySessionsServiceDbContext context)
    {
        _context = context;
    }

    public IQueryable<Session> GetAllSessions()
    {
        return _context.Sessions.AsQueryable();
    }

    public async Task<Session?> GetSessionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Sessions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Session>> GetAllUserSessionsAsync(string userId, CancellationToken cancellationToken)
    {
        return await _context.Sessions.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Session>> GetAllProductSessionsAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await _context.Sessions.Where(x => x.ProductId == productId).ToListAsync(cancellationToken);
    }

    public async Task<Session> CreateSessionAsync(Session session, CancellationToken cancellationToken)
    {
        var entityEntry = await _context.AddAsync(session, cancellationToken);

        return entityEntry.Entity;
    }

    public Session UpdateSession(Session session)
    {
        return _context.Update(session).Entity;
    }

    public void DeleteSession(Session session)
    {
        _context.Remove(session);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}