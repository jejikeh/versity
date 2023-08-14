using Application.Abstractions.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Infrastructure.Persistence.MongoRepositories;

public class ProductMongoRepository : IProductsRepository
{
    private readonly VersitySessionsServiceMongoDbContext _context;

    public ProductMongoRepository(VersitySessionsServiceMongoDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Product> GetProducts(int? skipCount, int? takeCount)
    {
        if (skipCount is null && takeCount is null)
        {
            return _context.Products
                .AsQueryable()
                .OrderBy(data => data.Title);
        }

        return _context.Products
            .AsQueryable()
            .OrderBy(data => data.Title)
            .Skip(skipCount ?? 0)
            .Take(takeCount ?? 10);
    }

    public async Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _context.Products.FindAsync(product => product.Id == id, cancellationToken: cancellationToken);
        
        return await result.SingleAsync(cancellationToken: cancellationToken);
    }

    public async Task<Product?> GetProductByExternalIdAsync(Guid externalId, CancellationToken cancellationToken)
    {
        var result = await _context.Products.FindAsync(product => product.ExternalId == externalId, cancellationToken: cancellationToken);

        return await result.SingleAsync(cancellationToken: cancellationToken);
    }

    public async Task<Product> CreateProductAsync(Product product, CancellationToken cancellationToken)
    {
        await _context.Products.InsertOneAsync(product, cancellationToken: cancellationToken);

        return product;
    }

    public async Task CreateRangeProductAsync(ICollection<Product> products, CancellationToken cancellationToken)
    {
        await _context.Products.InsertManyAsync(products, cancellationToken: cancellationToken);
    }

    public async Task<Product> UpdateProductAsync(Product product, CancellationToken cancellationToken)
    {
        var filterDefinition = Builders<Product>.Filter.Eq(prod => prod.Id, product.Id);
        await _context.Products.ReplaceOneAsync(filterDefinition, product, cancellationToken: cancellationToken);

        return await GetProductByIdAsync(product.Id, cancellationToken) ?? product;
    }

    public async void DeleteProduct(Product product)
    {
        var filterDefinition = Builders<Product>.Filter.Eq(prod => prod.Id, product.Id);
        await _context.Products.DeleteOneAsync(filterDefinition);
    }

    public async Task<List<Product>> ToListAsync(IQueryable<Product> products)
    {
        return await products.ToListAsync();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}