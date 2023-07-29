using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Bogus;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Services.KafkaConsumer.Abstractions;
using Infrastructure.Services.KafkaConsumer.Handlers.CreateProduct;
using Moq;
using Sessions.Tests.Application;

namespace Sessions.Tests.Infrastructure.Services.Kafka.Handlers;

public class CreateProductMessageHandlerTests
{
    private readonly Mock<IProductsRepository> _productsRepository;
    private readonly CreateProductMessageHandler _kafkaMessageHandler;

    public CreateProductMessageHandlerTests()
    {
        _productsRepository = new Mock<IProductsRepository>();
        var kafkaConsumerConfiguration = new Mock<IKafkaConsumerConfiguration>();
        
        kafkaConsumerConfiguration.Setup(x => x.CreateProductTopic).Returns("CreateProduct");
        kafkaConsumerConfiguration.Setup(x => x.DeleteProductTopic).Returns("DeleteProduct");
        
        _kafkaMessageHandler = new CreateProductMessageHandler(
            _productsRepository.Object,
            kafkaConsumerConfiguration.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenKeyNotForHandler()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();
        var message = new Faker().Lorem.Sentence();
        
        // Act
        await _kafkaMessageHandler.Handle(key, message, CancellationToken.None);
        
        // Assert
        _productsRepository.Verify(x => x.GetProductByExternalIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenKeyForHandlerIsSet()
    {
        // Arrange
        var key = "CreateProduct";
        var message = GenerateFakeJsonCreateProductMessage();
        _productsRepository.Setup(x => x.GetProductByExternalIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeProduct());
        
        // Act
        var act = async () => await _kafkaMessageHandler.Handle(key, message, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<ExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task Handle_ShouldCreateProductAndSaveChanges_WhenProductDoesNotExist()
    {
        // Arrange
        var key = "CreateProduct";
        var message = GenerateFakeJsonCreateProductMessage();
        _productsRepository.Setup(x => x.GetProductByExternalIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Product);
        
        // Act
        await _kafkaMessageHandler.Handle(key, message, CancellationToken.None);
        
        // Assert
        _productsRepository.Verify(repository => repository.CreateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _productsRepository.Verify(repository => repository.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static string GenerateFakeJsonCreateProductMessage()
    {
        return JsonSerializer.Serialize(GenerateFakeCreateProductMessage());
    }

    private static CreateProductMessage GenerateFakeCreateProductMessage()
    {
        return new Faker<CreateProductMessage>().CustomInstantiator(
            faker => new CreateProductMessage(
                Guid.NewGuid(),
                faker.Lorem.Sentence()));
    }
}