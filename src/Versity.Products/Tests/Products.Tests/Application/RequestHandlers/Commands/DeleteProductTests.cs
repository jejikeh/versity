using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.Exceptions;
using Application.RequestHandlers.Commands.DeleteProduct;
using Bogus;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Products.Tests.Application.RequestHandlers.Commands;

public class DeleteProductTests
{
    private readonly Mock<IVersityProductsRepository> _products;
    private readonly Mock<IProductProducerService> _productProducerService;

    public DeleteProductTests()
    {
        _products = new Mock<IVersityProductsRepository>();
        _productProducerService = new Mock<IProductProducerService>();
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenProductDoesNotExist()
    {
        _products.Setup(service =>
                service.GetProductByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Product);
        
        var command = new DeleteProductCommand(Guid.NewGuid());
        var handler = new DeleteProductCommandHandler(_products.Object, _productProducerService.Object);
        
        var act = async () => await handler.Handle(command, CancellationToken.None);
        
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldDeleteProduct_WhenProductExists()
    {
        var product = GenerateFakeProduct();
        _products.Setup(service =>
                service.GetProductByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        
        var command = new DeleteProductCommand(Guid.NewGuid());
        var handler = new DeleteProductCommandHandler(_products.Object, _productProducerService.Object);
        
        await handler.Handle(command, CancellationToken.None);
        
        _products.Verify(x => 
            x.DeleteProduct(product), Times.Once);
        
        _productProducerService.Verify(x =>
            x.DeleteProductProduce(It.Is<DeleteProductDto>(dto => dto.Id == product.Id), It.IsAny<CancellationToken>()), Times.Once);
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