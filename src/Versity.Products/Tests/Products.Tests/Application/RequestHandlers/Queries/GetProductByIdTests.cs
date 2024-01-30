using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.Queries.GetProductById;
using Bogus;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Products.Tests.Application.RequestHandlers.Queries;

public class GetProductByIdTests
{
    private readonly Mock<IVersityProductsRepository> _productsRepository;

    public GetProductByIdTests()
    {
        _productsRepository = new Mock<IVersityProductsRepository>();
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenProductDoesNotExist()
    {
        // Arrange
        var handler = new GetProductByIdQueryHandler(_productsRepository.Object);
        var request = new GetProductByIdQuery(Guid.NewGuid());
        
        // Act
        var act = async () => await handler.Handle(request, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task Handle_ShouldReturnProduct_WhenProductDoesExist()
    {
        // Arrange
        var handler = new GetProductByIdQueryHandler(_productsRepository.Object);
        var request = new GetProductByIdQuery(Guid.NewGuid());
        _productsRepository.Setup(repository =>
                repository.GetProductByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenerateFakeProduct);
        
        // Act
        var product = await handler.Handle(request, CancellationToken.None);
        
        // Assert
        product.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenIdIsValid()
    {
        // Arrange
        var validator = new GetProductByIdQueryValidator();
        var command = new GetProductByIdQuery(Guid.NewGuid());
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenIdIsInvalidValid()
    {
        // Arrange
        var validator = new GetProductByIdQueryValidator();
        var command = new GetProductByIdQuery(default);
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
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