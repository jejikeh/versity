using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Application.Abstractions.Repositories;

public interface IVersityUsersRepository
{
    public IQueryable<VersityUser?> GetAllUsersAsync();
    public Task<VersityUser?> GetUserAsync(string id);
    public Task<IList<string>> GetUserRolesAsync(VersityUser user);
    public Task<IdentityResult> CreateUserAsync(VersityUser user);
    public Task<IdentityResult> UpdateUserAsync(VersityUser user);
    public Task<IdentityResult> DeleteUserAsync(VersityUser user);
    public Task<IdentityResult> SetUserRoleAsync(VersityUser user, VersityRole role);
}