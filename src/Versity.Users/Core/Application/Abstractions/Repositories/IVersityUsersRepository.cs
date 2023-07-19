using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Application.Abstractions.Repositories;

public interface IVersityUsersRepository
{
    public IQueryable<VersityUser?> GetAllUsers();
    public Task<VersityUser?> GetUserByIdAsync(string id);
    public Task<VersityUser?> GetUserByEmailAsync(string email);
    public Task<IList<string>> GetUserRolesAsync(VersityUser user);
    public Task<IdentityResult> CreateUserAsync(VersityUser user, string password);
    public Task<IdentityResult> UpdateUserAsync(VersityUser user);
    public Task<IdentityResult> DeleteUserAsync(VersityUser user);
    public Task<IdentityResult> SetUserRoleAsync(VersityUser user, VersityRole role);
    public Task<bool> CheckPasswordAsync(VersityUser user, string requestPassword);
    public Task<IdentityResult> ResetPasswordAsync(VersityUser user, string token, string newPassword);
    public Task<string> GeneratePasswordResetTokenAsync(VersityUser user);
    public Task<List<VersityUser>> ToListAsync(IQueryable<VersityUser> users);
    public Task<IdentityResult> ConfirmUserEmail(VersityUser user, string code);
}