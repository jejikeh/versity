using Application.Abstractions.Repositories;
using Domain.Models;
using Infrastructure.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Persistence.Repositories;

public class CachedProductsRepository : IVersityProductsRepository
{
    private readonly IVersityProductsRepository _productsRepository;
    private readonly IDistributedCache _distributedCache;
    private readonly VersityProductsDbContext _context;

    public CachedProductsRepository(IVersityProductsRepository productsRepository, IDistributedCache distributedCache, VersityProductsDbContext context)
    {
        _productsRepository = productsRepository;
        _distributedCache = distributedCache;
        _context = context;
    }

    public IQueryable<Product> GetAllProducts()
    {
        return _distributedCache.GetOrCreateQueryable(
            $"products",
            () => _productsRepository.GetAllProducts());
    }

    public async Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _distributedCache.GetOrCreateAsync(
            _context,
            $"product-by-id-{id}",
            cancellationToken,
            async () => await _productsRepository.GetProductByIdAsync(id, cancellationToken));
    }

    public Task<Product> CreateProductAsync(Product product, CancellationToken cancellationToken)
    {
        return _productsRepository.CreateProductAsync(product, cancellationToken);
    }

    public Product UpdateProduct(Product product)
    {
        return _productsRepository.UpdateProduct(product);
    }

    public void DeleteProduct(Product product)
    {
        _productsRepository.DeleteProduct(product);
    }

    public Task<List<Product>> ToListAsync(IQueryable<Product> products)
    {
        return products is IAsyncEnumerable<Product> ? _productsRepository.ToListAsync(products) : Task.Run(products.ToList);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _productsRepository.SaveChangesAsync(cancellationToken);
    }
}