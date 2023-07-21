using Application.Behaviors;
using Application.Exceptions;
using Application.RequestHandlers.Commands.CreateProduct;
using Bogus;
using Domain.Models;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;

namespace Products.Tests.Application.Behaviors;

public class ValidationPipelineBehaviorTests
{
    private readonly IEnumerable<IValidator<CreateProductCommand>> _validators =
        new List<IValidator<CreateProductCommand>>()
        {
            new CreateProductCommandValidator()
        };
    
    [Fact]
    public async Task ValidationPipelineBehavior_ShouldThrowException_WhenValidationFails()
    {
        var pipeline = new ValidationPipelineBehavior<CreateProductCommand, Product>(_validators);
        
        var act = () => pipeline.Handle(GenerateFakeInvalidCreateProductCommand(), default , CancellationToken.None);

        await act.Should().ThrowAsync<ValidationExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task ValidationPipelineBehavior_ShouldInvokeNextPipeline_WhenValidationSucceeds()
    {
        var mockedPipeline = new Mock<IPipelineBehavior<CreateProductCommand, Product>>();
        
        var act = await GenerateValidationPipelineBehaviorBoilerplate(
            GenerateFakeCreateProductCommand(), 
            _validators, 
            () => mockedPipeline.Object.Handle(It.IsAny<CreateProductCommand>(), It.IsAny<RequestHandlerDelegate<Product>>(), It.IsAny<CancellationToken>()));

        mockedPipeline.Verify(x => x.Handle(It.IsAny<CreateProductCommand>(), It.IsAny<RequestHandlerDelegate<Product>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ValidationPipelineBehavior_ShouldThrowException_WhenValidationFailsInNextPipeline()
    {
        var act = () => GenerateValidationPipelineBehaviorBoilerplate<CreateProductCommand, Product>(
            GenerateFakeCreateProductCommand(), 
            _validators, 
            () => GenerateValidationPipelineBehaviorBoilerplate<CreateProductCommand, Product>(
                GenerateFakeInvalidCreateProductCommand(), 
                _validators));

        await act.Should().ThrowAsync<ValidationExceptionWithStatusCode>();
    }

    private Task<TResponse> GenerateValidationPipelineBehaviorBoilerplate<TRequest, TResponse>(
        TRequest command,
        IEnumerable<IValidator<TRequest>> validators,
        RequestHandlerDelegate<TResponse> next = default) where TRequest : IRequest<TResponse>
    {
        var pipeline = new ValidationPipelineBehavior<TRequest, TResponse>(validators);
        
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