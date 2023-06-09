using Application.Abstractions.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Persistence.Repositories;

public class VersityRoleRepository : IVersityRolesRepository
{
    private readonly RoleManager<IdentityRole> _context;

    public VersityRoleRepository(RoleManager<IdentityRole> context)
    {
        _context = context;
    }

    public async Task<IdentityResult> CreateRoleAsync(VersityRole role)
    {
        var roleString = role.ToString();
        if (!await _context.RoleExistsAsync(roleString))
            return await _context.CreateAsync(new IdentityRole(roleString));

        return IdentityResult.Failed();
    }

    public async Task<IdentityResult> DeleteRoleAsync(IdentityRole role)
    {
        return await _context.DeleteAsync(role);
    }
}