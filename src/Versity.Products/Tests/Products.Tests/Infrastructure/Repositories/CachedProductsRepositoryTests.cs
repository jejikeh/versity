using Application.Abstractions;
using Application.Abstractions.Repositories;
using Bogus;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Persistence.Repositories;
using Moq;

namespace Products.Tests.Infrastructure.Repositories;

public class CachedProductsRepositoryTests
{
    private readonly Mock<IVersityProductsRepository> _productsRepository;
    private readonly Mock<ICacheService> _distributedCache;

    public CachedProductsRepositoryTests()
    {
        _productsRepository = new Mock<IVersityProductsRepository>();
        _distributedCache = new Mock<ICacheService>();
    }

    [Fact]
    public void GetAllProducts_ShouldInvokeRepository_WhenValueIsNotInCache()
    {
        // Arrange
        var cachedProductsRepository = new CachedProductsRepository(_productsRepository.Object, _distributedCache.Object);

        // Act
        cachedProductsRepository.GetAllProducts();

        // Assert
        _productsRepository.Verify(x => x.GetAllProducts(), Times.Once);
    }
    
    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnAllRefreshTokens_WhenEmptyValueInCacheKey()
    {
        // Arrange
        var product = GenerateFakeProduct();
        var cachedProductsRepository = new CachedProductsRepository(_productsRepository.Object, _distributedCache.Object);

        _distributedCache.Setup(x =>
                x.GetOrCreateAsync(It.IsAny<string>(), It.IsAny<Func<Task<Product>?>>()))
            .ReturnsAsync(() => null);

        _productsRepository.Setup(x => 
                x.GetProductByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        
        // Act
        var result = await cachedProductsRepository.GetProductByIdAsync(Guid.NewGuid(), CancellationToken.None);
        
        // Assert
        result.Should().BeSameAs(product);
    }
    
    [Fact]
    public async Task CreateProductAsync_ShouldInvokeRepository_WhenValueIsNotInCache()
    {
        // Arrange
        var cachedProductsRepository = new CachedProductsRepository(_productsRepository.Object, _distributedCache.Object);

        // Act
        await cachedProductsRepository.CreateProductAsync(GenerateFakeProduct(), CancellationToken.None);

        // Assert
        _productsRepository.Verify(x => 
            x.CreateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public void UpdateProduct_ShouldInvokeRepository_WhenValueIsNotInCache()
    {
        // Arrange
        var cachedProductsRepository = new CachedProductsRepository(_productsRepository.Object, _distributedCache.Object);

        // Act
        cachedProductsRepository.UpdateProduct(GenerateFakeProduct());

        // Assert
        _productsRepository.Verify(x => 
                x.UpdateProduct(It.IsAny<Product>()),
            Times.Once);
    }
    
    [Fact]
    public async Task SaveChangesAsync_ShouldInvokeRepository_WhenValueIsNotInCache()
    {
        // Arrange
        var cachedProductsRepository = new CachedProductsRepository(_productsRepository.Object, _distributedCache.Object);

        // Act
        await cachedProductsRepository.SaveChangesAsync(CancellationToken.None);

        // Assert
        _productsRepository.Verify(x => 
                x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    private static Product GenerateFakeProduct()
    {
        return new Faker<Product>()
            .RuleFor(product => product.Id, f => Guid.NewGuid())
            .RuleFor(product => product.Author, f => f.Company.CompanyName())
            .RuleFor(product => product.Description, f => f.Lorem.Sentence())
            .RuleFor(product => product.Release, f => f.Date.PastDateOnly())
            .RuleFor(product => product.Title, f => f.Commerce.ProductName())
            .Generate();
    }
}