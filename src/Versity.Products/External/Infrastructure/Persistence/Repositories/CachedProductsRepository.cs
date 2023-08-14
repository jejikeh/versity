using Application.Abstractions;
using Application.Abstractions.Repositories;
using Domain.Models;

namespace Infrastructure.Persistence.Repositories;

public class CachedProductsRepository : IVersityProductsRepository
{
    private readonly IVersityProductsRepository _productsRepository;
    private readonly ICacheService _distributedCache;

    public CachedProductsRepository(IVersityProductsRepository productsRepository, ICacheService distributedCache)
    {
        _productsRepository = productsRepository;
        _distributedCache = distributedCache;
    }

    public Task<IEnumerable<Product>> GetProductsAsync(
        int? skipEntitiesCount, 
        int? takeEntitiesCount,
        CancellationToken cancellationToken)
    {
        return _productsRepository.GetProductsAsync(skipEntitiesCount, takeEntitiesCount, cancellationToken);
    }

    public async Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _distributedCache.GetOrCreateAsync(
            $"product-by-id-{id}",
            async () => await _productsRepository.GetProductByIdAsync(id, cancellationToken)) ?? 
               await _productsRepository.GetProductByIdAsync(id, cancellationToken);
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
        return _productsRepository.ToListAsync(products);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _productsRepository.SaveChangesAsync(cancellationToken);
    }
}