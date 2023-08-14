using Domain.Models;
using Domain.Models.SessionLogging;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;
 
public class VersitySessionsServiceSqlDbContext : DbContext
{
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<LogData> LogsData { get; set; }
    public DbSet<SessionLogs> SessionLogs { get; set; }

    public VersitySessionsServiceSqlDbContext(DbContextOptions<VersitySessionsServiceSqlDbContext> options) : base(options)
    {
    }
}