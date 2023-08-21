using Application.Abstractions.Repositories;
using Application.RequestHandlers.Queries.GetAllProducts;
using Bogus;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Products.Tests.Application.RequestHandlers.Queries;

public class GetAllProductsTests
{
    private readonly Mock<IVersityProductsRepository> _productsRepository;

    public GetAllProductsTests()
    {
        _productsRepository = new Mock<IVersityProductsRepository>();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllProducts_WhenCalled()
    {
        // Arrange
        var products = GenerateFakeProductsList();
        _productsRepository.Setup(productsRepository => 
                productsRepository.GetProductsAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(products));
        
        var handler = new GetAllProductsQueryHandler(_productsRepository.Object);
        
        // Act
        var result = await handler.Handle(new GetAllProductsQuery(1), CancellationToken.None);

        // Assert
        result.Count().Should().Be(products.Count());
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenPageIsValid()
    {
        // Arrange
        var validator = new GetAllProductsQueryValidator();
        var command = new GetAllProductsQuery(2);
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenPageIsInvalid()
    {
        // Arrange
        var validator = new GetAllProductsQueryValidator();
        var command = new GetAllProductsQuery(-1);
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }

    private static IEnumerable<Product> GenerateFakeProductsList()
    {
        return new Faker<Product>()
            .RuleFor(product => product.Id, f => Guid.NewGuid())
            .RuleFor(product => product.Author, f => f.Company.CompanyName())
            .RuleFor(product => product.Description, f => f.Lorem.Sentence())
            .RuleFor(product => product.Release, f => f.Date.PastDateOnly())
            .RuleFor(product => product.Title, f => f.Commerce.ProductName())
            .Generate(20);
    }
}