using Domain.Models;

namespace Application.Abstractions.Repositories;

public interface IProductsRepository
{
    public IQueryable<Product> GetAllProducts();
    public Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
    public Task<Product> CreateProductAsync(Product product, CancellationToken cancellationToken);
    public Product UpdateProduct(Product product);
    public void DeleteProduct(Product product);
    public Task SaveChangesAsync(CancellationToken cancellationToken);
}