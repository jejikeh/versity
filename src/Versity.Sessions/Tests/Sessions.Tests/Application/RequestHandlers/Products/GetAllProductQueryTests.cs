using Application.Abstractions.Repositories;
using Application.RequestHandlers.Products.Queries.GetAllProducts;
using Domain.Models;
using Moq;

namespace Sessions.Tests.Application.RequestHandlers.Products;

public class GetAllProductQueryTests
{
    private readonly Mock<IProductsRepository> _mockProductsRepository;

    public GetAllProductQueryTests()
    {
        _mockProductsRepository = new Mock<IProductsRepository>();
    }

    [Fact]
    public async Task Handle_ShouldReturnsProducts_WhenCalled()
    {
        // Arrange
        var products = FakeDataGenerator.GenerateFakeProducts(15);
        _mockProductsRepository.Setup(repository => repository.GetAllProducts()).Returns(products.AsQueryable());
        var handler = new GetAllProductQueryHandler(_mockProductsRepository.Object);
        
        // Act
        await handler.Handle(new GetAllProductsQuery(2), CancellationToken.None);
        
        // Assert
        _mockProductsRepository.Verify(repository => 
            repository.ToListAsync(
                It.Is<IQueryable<Product>>(queryable => queryable.Count() == 5)), 
            Times.Once());
    }
    
    [Fact]
    public async Task Handle_ShouldReturnsCorrectCountOfProducts_WhenCalledWithPage()
    {
        // Arrange
        var products = FakeDataGenerator.GenerateFakeProducts(15);
        _mockProductsRepository.Setup(repository => repository.GetAllProducts()).Returns(products.AsQueryable());
        var handler = new GetAllProductQueryHandler(_mockProductsRepository.Object);
        
        // Act
        await handler.Handle(new GetAllProductsQuery(1), CancellationToken.None);
        
        // Assert
        _mockProductsRepository.Verify(repository => 
                repository.ToListAsync(
                    It.Is<IQueryable<Product>>(queryable => queryable.Count() == 10)), 
            Times.Once());
    }
}