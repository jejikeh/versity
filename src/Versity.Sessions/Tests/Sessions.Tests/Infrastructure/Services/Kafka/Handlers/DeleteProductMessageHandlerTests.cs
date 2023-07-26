﻿using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Abstractions.Repositories;
using Application.Exceptions;
using Bogus;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Services.KafkaConsumer.Abstractions;
using Infrastructure.Services.KafkaConsumer.Handlers.CreateProduct;
using Infrastructure.Services.KafkaConsumer.Handlers.DeleteProduct;
using Moq;
using Sessions.Tests.Application;

namespace Sessions.Tests.Infrastructure.Services.Kafka.Handlers;

public class DeleteProductMessageHandlerTests
{
    private readonly Mock<IProductsRepository> _productsRepository;
    private readonly Mock<IKafkaConsumerConfiguration> _kafkaConsumerConfiguration;

    public DeleteProductMessageHandlerTests()
    {
        _productsRepository = new Mock<IProductsRepository>();
        _kafkaConsumerConfiguration = new Mock<IKafkaConsumerConfiguration>();
        
        _kafkaConsumerConfiguration.Setup(x => x.CreateProductTopic).Returns("CreateProduct");
        _kafkaConsumerConfiguration.Setup(x => x.DeleteProductTopic).Returns("DeleteProduct");
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenKeyNotForHandler()
    {
        // Arrange
        var kafkaMessageHandler = new DeleteProductMessageHandler(
            _productsRepository.Object,
            _kafkaConsumerConfiguration.Object);
        
        var key = Guid.NewGuid().ToString();
        var message = new Faker().Lorem.Sentence();
        
        // Act
        await kafkaMessageHandler.Handle(key, message, CancellationToken.None);
        
        // Assert
        _productsRepository.Verify(x => x.GetProductByExternalIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenKeyForHandlerIsSet()
    {
        // Arrange
        var kafkaMessageHandler = new DeleteProductMessageHandler(
            _productsRepository.Object,
            _kafkaConsumerConfiguration.Object);
        
        var key = "DeleteProduct";
        var message = GenerateFakeJsonCreateProductMessage();
        _productsRepository.Setup(x => x.GetProductByExternalIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Product);
        
        // Act
        var act = async () => await kafkaMessageHandler.Handle(key, message, CancellationToken.None);
        
        // Assert
        await act.Should().ThrowAsync<ExceptionWithStatusCode>();
    }
    
    [Fact]
    public async Task Handle_ShouldDeleteProductAndSaveChanges_WhenProductDoesNotExist()
    {
        // Arrange
        var kafkaMessageHandler = new DeleteProductMessageHandler(
            _productsRepository.Object,
            _kafkaConsumerConfiguration.Object);
        
        var key = "DeleteProduct";
        var message = GenerateFakeJsonCreateProductMessage();
        _productsRepository.Setup(x => x.GetProductByExternalIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeDataGenerator.GenerateFakeProduct());
        
        // Act
        await kafkaMessageHandler.Handle(key, message, CancellationToken.None);
        
        // Assert
        _productsRepository.Verify(repository => repository.DeleteProduct(It.IsAny<Product>()), Times.Once);
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