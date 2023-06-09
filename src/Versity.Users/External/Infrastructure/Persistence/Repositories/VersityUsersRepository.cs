using Application.Abstractions;
using Application.Abstractions.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Persistence.Repositories;

public class VersityUsersRepository : IVersityUsersRepository
{
    private readonly UserManager<VersityUser> _context;

    public VersityUsersRepository(UserManager<VersityUser> context)
    {
        _context = context;
    }

    public IQueryable<VersityUser?> GetAllUsersAsync()
    {
        return _context.Users;
    }

    public async Task<VersityUser?> GetUserAsync(string id)
    {
        return await _context.FindByIdAsync(id);
    }

    public async Task<IList<string>> GetUserRolesAsync(VersityUser user)
    {
        return await _context.GetRolesAsync(user);
    }

    public async Task<IdentityResult> CreateUserAsync(VersityUser user)
    {
        return await _context.CreateAsync(user);
    }

    public async Task<IdentityResult> UpdateUserAsync(VersityUser user)
    {
        return await _context.UpdateAsync(user);
    }

    public async Task<IdentityResult>  DeleteUserAsync(VersityUser user)
    {
        return await _context.DeleteAsync(user);
    }

    public async Task<IdentityResult> SetUserRoleAsync(VersityUser user, VersityRole role)
    {
        return await _context.AddToRoleAsync(user, role.ToString());
    }
}