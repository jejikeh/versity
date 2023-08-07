using Application.Abstractions.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductsRepository
{
    private readonly VersitySessionsServiceDbContext _context;

    public ProductRepository(VersitySessionsServiceDbContext context)
    {
        _context = context;
    }

    public IQueryable<Product> GetAllProducts()
    {
        return _context.Products.AsQueryable();
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