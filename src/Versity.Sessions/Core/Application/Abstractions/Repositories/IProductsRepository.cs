using Domain.Models;

namespace Application.Abstractions.Repositories;

public interface IProductsRepository
{
    public IEnumerable<Product> GetProducts(int? skipCount, int? takeCount);
    public Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<Product?> GetProductByExternalIdAsync(Guid externalId, CancellationToken cancellationToken);
    public Task<Product> CreateProductAsync(Product product, CancellationToken cancellationToken);
    public Task CreateRangeProductAsync(ICollection<Product> products, CancellationToken cancellationToken);
    public void DeleteProduct(Product product);
    public Task SaveChangesAsync(CancellationToken cancellationToken);
}