using System.Reflection;
using Domain.Models;
using Domain.Models.SessionLogging;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class VersitySessionsServiceDbContext : DbContext
{
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<LogData> LogsData { get; set; }
    public DbSet<SessionLogs> SessionLogs { get; set; }

    public VersitySessionsServiceDbContext(DbContextOptions<VersitySessionsServiceDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}