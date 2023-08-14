using Application.Abstractions.Repositories;
using Application.Common;
using Application.RequestHandlers.Products.Queries.GetAllProducts;
using Domain.Models;
using FluentAssertions;
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
        const int entriesCount = PageFetchSettings.ItemsOnPage + 5;
        var products = FakeDataGenerator.GenerateFakeProducts(entriesCount);
        _mockProductsRepository.Setup(repository => repository.GetProducts(
            It.IsAny<int?>(), It.IsAny<int?>()))
            .Returns(products);
        var handler = new GetAllProductQueryHandler(_mockProductsRepository.Object);
        
        // Act
        var results = await handler.Handle(new GetAllProductsQuery(2), CancellationToken.None);
        
        // Assert
        results.Count().Should().Be(entriesCount);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnsCorrectCountOfProducts_WhenCalledWithPage()
    {
        // Arrange
        const int entriesCount = PageFetchSettings.ItemsOnPage + 5;
        var products = FakeDataGenerator.GenerateFakeProducts(entriesCount);
        _mockProductsRepository.Setup(repository => repository.GetProducts(
            It.IsAny<int?>(), It.IsAny<int?>()))
            .Returns(products);
        var handler = new GetAllProductQueryHandler(_mockProductsRepository.Object);
        
        // Act
        var result = await handler.Handle(new GetAllProductsQuery(1), CancellationToken.None);
        
        // Assert
        result.Count().Should().Be(entriesCount);
    }
}