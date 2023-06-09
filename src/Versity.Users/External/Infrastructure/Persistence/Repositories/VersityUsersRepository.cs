using Application.Abstractions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class VersityUsersRepository : IVersityUsersRepository
{
    private readonly VersityUsersDbContext _context;

    public VersityUsersRepository(VersityUsersDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VersityUser?>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        return await _context.VersityUsers.ToListAsync(cancellationToken);
    }

    public async Task<VersityUser?> GetUserAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.VersityUsers.SingleOrDefaultAsync(x => x.Id == id.ToString(), cancellationToken);
    }

    public async Task CreateUserAsync(VersityUser user, CancellationToken cancellationToken)
    {
        await _context.AddAsync(user, cancellationToken);
    }

    public void UpdateUserAsync(VersityUser user)
    {
        _context.Update(user);
    }

    public void DeleteUserAsync(VersityUser user)
    {
        _context.Remove(user);
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}