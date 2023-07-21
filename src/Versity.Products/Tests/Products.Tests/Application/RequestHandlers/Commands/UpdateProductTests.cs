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
    private readonly Mock<IVersityProductsRepository> _products;

    public UpdateProductTests()
    {
        _products = new Mock<IVersityProductsRepository>();
    }

    [Fact]
    public async Task RequestHandler_ShouldUpdateProduct_WhenProductExists()
    {
        // Arrange
        var command = GenerateFakeUpdateProductCommand();
        var handler = new UpdateProductCommandHandler(_products.Object);
        _products.Setup(productsRepository =>
                productsRepository.GetProductByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenerateFakeProduct);
        
        // Act
        var product = await handler.Handle(command, CancellationToken.None);

        // Assert
        _products.Verify(productsRepository => 
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
        var command = GenerateFakeUpdateProductCommand();
        var handler = new UpdateProductCommandHandler(_products.Object);
        _products.Setup(x =>
                x.GetProductByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Product);
        
        var act = async () => await handler.Handle(command, CancellationToken.None);
        
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