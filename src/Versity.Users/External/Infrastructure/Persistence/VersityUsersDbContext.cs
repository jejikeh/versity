using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class VersityUsersDbContext : IdentityDbContext<VersityUser>
{
    public VersityUsersDbContext(DbContextOptions options) : base(options)
    {
        
    }
}