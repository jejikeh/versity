using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class VersitySessionsServiceDbContext : DbContext
{
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Product> Products { get; set; }

    public VersitySessionsServiceDbContext(DbContextOptions<VersitySessionsServiceDbContext> options) : base(options)
    {
    }
}