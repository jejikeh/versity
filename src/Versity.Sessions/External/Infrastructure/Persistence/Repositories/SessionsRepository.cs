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
        return _context.Sessions
            .Include(x => x.Product)
            .Include(x => x.Logs)
            .AsQueryable();
    }

    public async Task<Session?> GetSessionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Sessions
            .Include(x => x.Product)
            .Include(x => x.Logs)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public IQueryable<Session> GetAllUserSessions(string userId)
    {
        return _context.Sessions.Where(x => x.UserId == userId);
    }

    public IQueryable<Session> GetAllProductSessions(Guid productId)
    {
        return _context.Sessions.Where(x => x.Product.Id == productId);
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

    public async Task<List<Session>> ToListAsync(IQueryable<Session> sessions)
    {
        return await sessions.ToListAsync();
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