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

    public IQueryable<Product> GetAllProducts()
    {
        return _context.Products.AsQueryable();
    }

    public async Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Product> CreateProductAsync(Product product, CancellationToken cancellationToken = default)
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

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}