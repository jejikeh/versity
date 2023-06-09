using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Application.Abstractions.Repositories;

public interface IVersityRolesRepository
{
    public Task<IdentityResult> CreateRoleAsync(VersityRole role);
    public Task<IdentityResult> DeleteRoleAsync(IdentityRole role);
}