using Domain.Models;

namespace Application.Abstractions.Repositories;

public interface IProductRepository
{
    public Task<IQueryable<Product>> GetAllProducts();
    public Task<Product?> GetProductById(Guid id);
    public Task<Product> CreateProductAsync(Product product);
    public Task<Product> UpdateProductAsync(Product product);
    public Task DeleteProductAsync(Product product);
    public Task SaveChangesAsync();
}