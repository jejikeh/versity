using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class VersityProductsDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public VersityProductsDbContext(DbContextOptions<VersityProductsDbContext> options) : base(options)
    {
    }
}