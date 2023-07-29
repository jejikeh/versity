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
    private readonly Mock<IVersityProductsRepository> _productsRepository;
    private readonly Mock<IProductProducerService> _productProducerService;
    private readonly DeleteProductCommandHandler _deleteProductCommandHandler;

    public DeleteProductTests()
    {
        _productsRepository = new Mock<IVersityProductsRepository>();
        _productProducerService = new Mock<IProductProducerService>();
        _deleteProductCommandHandler = new DeleteProductCommandHandler(_productsRepository.Object, _productProducerService.Object);
    }

    [Fact]
    public async Task RequestHandler_ShouldThrowException_WhenProductDoesNotExist()
    {
        // Arrange
        _productsRepository.Setup(service =>
                service.GetProductByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Product);
        
        var command = new DeleteProductCommand(Guid.NewGuid());
        
        // Act
        var act = async () => await _deleteProductCommandHandler.Handle(command, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<NotFoundExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldDeleteProduct_WhenProductExists()
    {
        // Arrange
        var product = GenerateFakeProduct();
        _productsRepository.Setup(service =>
                service.GetProductByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        
        var command = new DeleteProductCommand(Guid.NewGuid());
        
        // Act
        await _deleteProductCommandHandler.Handle(command, CancellationToken.None);
        
        // Assert
        _productsRepository.Verify(x => 
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