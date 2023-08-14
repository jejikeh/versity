using Application.Abstractions.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Infrastructure.Persistence.SqlRepositories;

public class SessionsSqlRepository : ISessionsRepository
{
    private readonly VersitySessionsServiceSqlDbContext _context;

    public SessionsSqlRepository(VersitySessionsServiceSqlDbContext context)
    {
        _context = context;
    }
    

    public async Task<Session?> GetSessionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Sessions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public IQueryable<Session> GetAllUserSessions(string userId)
    {
        return _context.Sessions.Where(x => x.UserId == userId);
    }

    public IQueryable<Session> GetAllProductSessions(Guid productId)
    {
        return _context.Sessions.Where(x => x.ProductId == productId);
    }

    public async Task<Session> CreateSessionAsync(Session session, CancellationToken cancellationToken)
    {
        var entityEntry = await _context.AddAsync(session, cancellationToken);

        return entityEntry.Entity;
    }

    public async Task CreateSessionRangeAsync(ICollection<Session> sessions, CancellationToken cancellationToken)
    {
        await _context.AddRangeAsync(sessions, cancellationToken);
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

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}