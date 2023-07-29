using Application.Abstractions.Repositories;
using Application.Exceptions;
using Application.RequestHandlers.Commands.UpdateProduct;
using Bogus;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Products.Tests.Application.RequestHandlers.Commands;

public class UpdateProductTests
{
    private readonly Mock<IVersityProductsRepository> _productsRepository;
    private readonly UpdateProductCommandHandler _updateProductCommandHandler;

    public UpdateProductTests()
    {
        _productsRepository = new Mock<IVersityProductsRepository>();
        _updateProductCommandHandler = new UpdateProductCommandHandler(_productsRepository.Object);
    }

    [Fact]
    public async Task RequestHandler_ShouldUpdateProduct_WhenProductExists()
    {
        // Arrange
        var command = GenerateFakeUpdateProductCommand();
        _productsRepository.Setup(productsRepository =>
                productsRepository.GetProductByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenerateFakeProduct);
        
        // Act
        var product = await _updateProductCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        _productsRepository.Verify(productsRepository => 
            productsRepository.UpdateProduct(It.Is<Product>(
                productParameter => 
                    productParameter.Author == command.Author &&
                    productParameter.Description == command.Description &&
                    productParameter.Release == command.Release &&
                    productParameter.Title == command.Title)), Times.Once);
    }
    
    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenProductDoesNotExist()
    {
        // Arrange
        var command = GenerateFakeUpdateProductCommand();
        _productsRepository.Setup(x =>
                x.GetProductByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Product);
        
        // Act
        var act = async () => await _updateProductCommandHandler.Handle(command, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }

    private static UpdateProductCommand GenerateFakeUpdateProductCommand()
    {
        return new Faker<UpdateProductCommand>()
            .CustomInstantiator(f => new UpdateProductCommand(
                Guid.NewGuid(),
                f.Commerce.ProductName(),
                f.Commerce.ProductDescription(),
                f.Company.CompanyName(),
                f.Date.PastDateOnly()))
            .Generate();
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