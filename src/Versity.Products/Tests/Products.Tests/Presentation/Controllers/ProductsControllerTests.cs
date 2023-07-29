using Application.Dtos;
using Application.RequestHandlers.Commands.CreateProduct;
using Application.RequestHandlers.Commands.UpdateProduct;
using Application.RequestHandlers.Queries.GetAllProducts;
using Application.RequestHandlers.Queries.GetProductById;
using Bogus;
using Domain.Models;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;

namespace Products.Tests.Presentation.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<ISender> _sender;
    private readonly ProductsController _productsController;

    public ProductsControllerTests()
    {
        _sender = new Mock<ISender>();
        _productsController = new ProductsController(_sender.Object);
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnOk_WhenModelIsValid()
    {
        // Arrange
        _sender.Setup(x =>
                x.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenerateFakeProductsList());
        
        // Act
        var response = await _productsController.GetAllProducts(1, CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task GetProductById_ShouldReturnOk_WhenModelIsValid()
    {
        // Arrange
        _sender.Setup(x =>
                x.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenerateFakeProduct());
        
        // Act
        var response = await _productsController.GetProductById(Guid.NewGuid(), CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task DeleteProductById_ShouldReturnOk_WhenModelIsValid()
    {
        // Act
        var response = await _productsController.DeleteProductById(Guid.NewGuid(), CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkResult>();
    }
    
    [Fact]
    public async Task CreateProduct_ShouldReturnOk_WhenModelIsValid()
    {
        // Arrange
        _sender.Setup(x =>
                x.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenerateFakeProduct());
        
        // Act
        var response = await _productsController.CreateProduct(GenerateFakeCreateProductCommand(), CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async Task UpdateProduct_ShouldReturnOk_WhenModelIsValid()
    {
        // Arrange
        _sender.Setup(x =>
                x.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GenerateFakeProduct());
        
        // Act
        var response = await _productsController.UpdateProduct(Guid.NewGuid(), GenerateFakeUpdateProductCommand(), CancellationToken.None);
        
        // Assert
        response.Should().BeOfType<OkObjectResult>();
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
    
    private static UpdateProductDto GenerateFakeUpdateProductCommand()
    {
        return new Faker<UpdateProductDto>()
            .CustomInstantiator(f => new UpdateProductDto(
                f.Commerce.ProductName(),
                f.Commerce.ProductDescription(),
                f.Company.CompanyName(),
                f.Date.PastDateOnly()))
            .Generate();
    }
}