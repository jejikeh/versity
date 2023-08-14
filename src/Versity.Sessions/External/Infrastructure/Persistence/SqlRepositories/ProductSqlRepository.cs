using Application.Abstractions.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Infrastructure.Persistence.SqlRepositories;

public class ProductSqlRepository : IProductsRepository
{
    private readonly VersitySessionsServiceSqlDbContext _context;

    public ProductSqlRepository(VersitySessionsServiceSqlDbContext context)
    {
        _context = context;
    }
    
    public IEnumerable<Product> GetProducts(int? skipCount, int? takeCount)
    {
        if (skipCount is null && takeCount is null)
        {
            return _context.Products
                .AsQueryable()
                .OrderBy(data => data.Title)
                .ToList();
        }
        
        return _context.Products
            .AsQueryable()
            .OrderBy(data => data.Title)
            .Skip(skipCount ?? 0)
            .Take(takeCount ?? 10)
            .ToList();
    }

    public async Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Product?> GetProductByExternalIdAsync(Guid externalId, CancellationToken cancellationToken)
    {
        return await _context.Products.FirstOrDefaultAsync(x => x.ExternalId == externalId, cancellationToken);
    }

    public async Task<Product> CreateProductAsync(Product product, CancellationToken cancellationToken)
    {
        var entityEntry = await _context.AddAsync(product, cancellationToken);

        return entityEntry.Entity;
    }

    public async Task CreateRangeProductAsync(ICollection<Product> products, CancellationToken cancellationToken)
    {
        await _context.AddRangeAsync(products, cancellationToken);
    }

    public void DeleteProduct(Product product)
    {
        _context.Remove(product);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}