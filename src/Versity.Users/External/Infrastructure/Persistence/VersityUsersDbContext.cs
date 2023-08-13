using System.Reflection;
using Domain.Models;
using Infrastructure.Persistence.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence;

public class VersityUsersDbContext : IdentityDbContext<VersityUser>
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    private readonly IConfiguration _configuration;

    public VersityUsersDbContext(
        DbContextOptions options,
        IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // since now im getting credentials from UserSecrets, i need manually pass IConfiguration
        modelBuilder.ApplyConfiguration(new AdminSeederConfiguration(_configuration));
    }
}