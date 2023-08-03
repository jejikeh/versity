using Application.Abstractions.Repositories;
using Bogus;
using Domain.Models;

namespace Products.Tests.Integrations.Helpers;

public static class ProductSeeder
{
    public static async Task<Product> SeedProductDataAsync(IVersityProductsRepository repository, Func<Product, Product>? changeProductAction = null)
    {
        var faker = new Faker();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Title = faker.Commerce.Product(),
            Description = faker.Commerce.ProductDescription(),
            Author = faker.Company.CompanyName(),
            Release = faker.Date.PastDateOnly()
        };

        changeProductAction?.Invoke(product);

        await repository.CreateProductAsync(product, CancellationToken.None);
        await repository.SaveChangesAsync(CancellationToken.None);
         
        return product;
    }
}