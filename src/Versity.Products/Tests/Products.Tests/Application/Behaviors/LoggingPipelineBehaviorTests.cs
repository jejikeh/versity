using Application.Behaviors;
using Application.RequestHandlers.Commands.CreateProduct;
using Bogus;
using Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Products.Tests.Application.Behaviors;

public class LoggingPipelineBehaviorTests
{
    private readonly Mock<ILogger<LoggingPipelineBehavior<CreateProductCommand, Product>>> _loggerMock;

    public LoggingPipelineBehaviorTests()
    {
        _loggerMock = new Mock<ILogger<LoggingPipelineBehavior<CreateProductCommand, Product>>>();
    }
    
    [Fact]
    public async Task ValidationPipelineBehavior_ShouldInvokeNextPipeline_WhenDelegateIsValid()
    {
        // Arrange
        var mockedPipeline = new Mock<IPipelineBehavior<CreateProductCommand, Product>>();
        
        // Act
        var act = await GenerateLoggingPipelineBehaviorBoilerplate(
            GenerateFakeCreateProductCommand(), 
            () => mockedPipeline.Object.Handle(It.IsAny<CreateProductCommand>(), It.IsAny<RequestHandlerDelegate<Product>>(), It.IsAny<CancellationToken>()));

        // Assert
        mockedPipeline.Verify(x => x.Handle(It.IsAny<CreateProductCommand>(), It.IsAny<RequestHandlerDelegate<Product>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    private Task<Product> GenerateLoggingPipelineBehaviorBoilerplate(
        CreateProductCommand command,
        RequestHandlerDelegate<Product> next = default)
    {
        var pipeline = new LoggingPipelineBehavior<CreateProductCommand, Product>(_loggerMock.Object);
        
        return pipeline.Handle(command, next , CancellationToken.None);
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
}