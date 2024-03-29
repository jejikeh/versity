﻿using Application.Abstractions.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class VersityUsersRepository : IVersityUsersRepository
{
    private readonly UserManager<VersityUser> _userManager;

    public VersityUsersRepository(UserManager<VersityUser> userManager)
    {
        _userManager = userManager;
    }

    public IQueryable<VersityUser?> GetAllUsers()
    {
        return _userManager.Users;
    }

    public async Task<VersityUser?> GetUserByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<VersityUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<IList<string>> GetUserRolesAsync(VersityUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<IdentityResult> CreateUserAsync(VersityUser user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> UpdateUserAsync(VersityUser user)
    {
        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult>  DeleteUserAsync(VersityUser user)
    {
        return await _userManager.DeleteAsync(user);
    }

    public async Task<IdentityResult> SetUserRoleAsync(VersityUser user, VersityRole role)
    {
        return await _userManager.AddToRoleAsync(user, role.ToString());
    }

    public async Task<bool> CheckPasswordAsync(VersityUser user, string requestPassword)
    {
        return await _userManager.CheckPasswordAsync(user, requestPassword);
    }
    
    public async Task<List<VersityUser>> ToListAsync(IQueryable<VersityUser> users)
    {
        return await users.ToListAsync();
    }

    public async Task<IdentityResult> ConfirmUserEmail(VersityUser user, string code)
    {
        return await _userManager.ConfirmEmailAsync(user, code);
    }

    public async Task<IdentityResult> ResetPasswordAsync(VersityUser user, string token, string newPassword)
    {
        return await _userManager.ResetPasswordAsync(user, token, newPassword);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(VersityUser user)
    {
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }
}