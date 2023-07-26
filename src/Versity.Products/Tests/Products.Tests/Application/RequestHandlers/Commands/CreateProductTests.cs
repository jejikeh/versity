using Application.Abstractions;
using Application.Abstractions.Repositories;
using Application.Dtos;
using Application.RequestHandlers.Commands.CreateProduct;
using Bogus;
using Domain.Models;
using FluentAssertions;
using Moq;

namespace Products.Tests.Application.RequestHandlers.Commands;

public class CreateProductTests
{
    private readonly Mock<IVersityProductsRepository> _products;
    private readonly Mock<IProductProducerService> _productProducerService;

    public CreateProductTests()
    {
        _products = new Mock<IVersityProductsRepository>();
        _productProducerService = new Mock<IProductProducerService>();
    }
    
    [Fact]
    public async Task RequestHandler_ShouldCreateProduct_WhenRequestIsValid()
    {
        // Arrange
        var command = GenerateFakeCreateProductCommand();
        var handler = new CreateProductCommandHandler(_products.Object, _productProducerService.Object);
        var product = new Product()
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            Description = command.Description,
            Author = command.Author,
            Release = command.Release
        };
        
        _products.Setup(service => 
                service.CreateProductAsync(
                    It.IsAny<Product>(), 
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await handler.Handle(command, default);
        
        // Assert
        _productProducerService.Verify(service =>
            service.CreateProductProduce(It.Is<CreateProductProduceDto>(productDto => productDto.Id == product.Id),
                It.IsAny<CancellationToken>()));
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationError_WhenModelIsInvalid()
    {
        // Arrange
        var validator = new CreateProductCommandValidator();
        var command = GenerateFakeInvalidCreateProductCommand();
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeFalse();
    }
    
    [Fact]
    public async Task Validation_ShouldReturnValidationSuccess_WhenModelIsValid()
    {
        // Arrange
        var validator = new CreateProductCommandValidator();
        var command = GenerateFakeCreateProductCommand();
        
        // Act
        var result = await validator.ValidateAsync(command);
        
        // Assert
        result.IsValid.Should().BeTrue();
    }

    private static CreateProductCommand GenerateFakeCreateProductCommand()
    {
        return new Faker<CreateProductCommand>().CustomInstantiator(f => 
                new CreateProductCommand(
                    f.Commerce.ProductName(), 
                    f.Commerce.ProductDescription(), 
                    f.Company.CompanyName(), 
                    f.Date.PastDateOnly()))
            .Generate();
    }
    
    private static CreateProductCommand GenerateFakeInvalidCreateProductCommand()
    {
        var random = new Random();
        return new Faker<CreateProductCommand>().CustomInstantiator(f => 
                new CreateProductCommand(
                    random.Next(2) == 1 ? f.Commerce.ProductName() : string.Empty, 
                    random.Next(2) == 1 ? f.Commerce.ProductDescription() : "Small desc", 
                    random.Next(2) == 1 ? f.Company.CompanyName() : string.Empty, 
                    random.Next(2) == 1 ? f.Date.PastDateOnly() : default))
            .Generate();
    }
}