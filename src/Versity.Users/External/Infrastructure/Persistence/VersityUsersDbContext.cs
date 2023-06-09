using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class VersityUsersDbContext : IdentityDbContext
{
    public DbSet<VersityUser?> VersityUsers { get; set; }

    public VersityUsersDbContext(DbContextOptions<VersityUsersDbContext> options) : base(options)
    {
        
    }
}