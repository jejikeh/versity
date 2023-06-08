using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Versity.Users.Core.Domain.Models;

namespace Versity.Users.Persistence;

public class VersityUsersDbContext : IdentityDbContext
{
    public DbSet<VersityUser?> VersityUsers { get; set; }

    public VersityUsersDbContext(DbContextOptions<VersityUsersDbContext> options) : base(options)
    {
        
    }
}