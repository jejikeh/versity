using Application.Abstractions.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class VersityProductsRepository : IVersityProductsRepository
{
    private readonly VersityProductsDbContext _context;

    public VersityProductsRepository(VersityProductsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(
        int? skipEntitiesCount, 
        int? takeEntitiesCount,
        CancellationToken cancellationToken)
    {
        if (skipEntitiesCount is null && takeEntitiesCount is null)
        {
            return await _context.Products
                .AsQueryable()
                .OrderBy(refreshToken => refreshToken.Release)
                .ToListAsync(cancellationToken);
        }
        
        return await _context.Products
            .AsQueryable()
            .OrderBy(refreshToken => refreshToken.Release)
            .Skip(skipEntitiesCount ?? 0)
            .Take(takeEntitiesCount ?? 10)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Product> CreateProductAsync(Product product, CancellationToken cancellationToken)
    {
        var entityEntry = await _context.AddAsync(product, cancellationToken);
        
        return entityEntry.Entity;
    }

    public Product UpdateProduct(Product product)
    {
        return _context.Update(product).Entity;
    }

    public void DeleteProduct(Product product)
    {
        _context.Remove(product);
    }

    public async Task<List<Product>> ToListAsync(IQueryable<Product> products)
    {
        return await products.ToListAsync();
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}