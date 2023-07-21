using Application.Dtos;
using Bogus;
using Infrastructure.Configurations;
using Infrastructure.Services;
using KafkaFlow;
using KafkaFlow.Producers;
using Microsoft.Extensions.Logging;
using Moq;

namespace Products.Tests.Infrastructure.Services;

public class KafkaProductProducerServiceTests
{
    private readonly Mock<IProducerAccessor> _producerAccessor;
    private readonly Mock<ILogger<KafkaProductProducerService>> _logger;
    private readonly Mock<IKafkaProducerConfiguration> _kafkaProducerConfiguration;
    private readonly Mock<IMessageProducer> _messageProducer;

    public KafkaProductProducerServiceTests()
    {
        _producerAccessor = new Mock<IProducerAccessor>();
        _logger = new Mock<ILogger<KafkaProductProducerService>>();
        _kafkaProducerConfiguration = new Mock<IKafkaProducerConfiguration>();
        _messageProducer = new Mock<IMessageProducer>();
        
        _kafkaProducerConfiguration.Setup(x => 
            x.ProducerName).Returns("ProducerName");
        _kafkaProducerConfiguration.Setup(x => 
            x.CreateProductTopic).Returns("CreateProduct");
        _kafkaProducerConfiguration.Setup(x => 
            x.DeleteProductTopic).Returns("DeleteProduct");
    }

    [Fact]
    public async Task CreateProductProduce_ShouldInvokeProducer_WhenIsCalled()
    {
        // Arrange
        var createProductProduceDto = GenerateCreateProductProduceDto();
        var producer = new KafkaProductProducerService(_producerAccessor.Object, _logger.Object, _kafkaProducerConfiguration.Object);
        _producerAccessor.Setup(x => 
                x.GetProducer(It.IsAny<string>()))
            .Returns(_messageProducer.Object);
        
        // Act
        await producer.CreateProductProduce(createProductProduceDto, CancellationToken.None);
        
        // Assert
        _producerAccessor.Verify(x => x.GetProducer("ProducerName"), Times.Once);
    }
    
    [Fact]
    public async Task DeleteProductProduce_ShouldInvokeProducer_WhenIsCalled()
    {
        // Arrange
        var createProductProduceDto = GenerateCreateProductProduceDto();
        var producer = new KafkaProductProducerService(_producerAccessor.Object, _logger.Object, _kafkaProducerConfiguration.Object);
        _producerAccessor.Setup(x => 
                x.GetProducer(It.IsAny<string>()))
            .Returns(_messageProducer.Object);
        
        // Act
        await producer.DeleteProductProduce(new DeleteProductDto(Guid.NewGuid()), CancellationToken.None);
        
        // Assert
        _producerAccessor.Verify(x => x.GetProducer("ProducerName"), Times.Once);
    }

    private static CreateProductProduceDto GenerateCreateProductProduceDto()
    {
        return new Faker<CreateProductProduceDto>().CustomInstantiator(faker =>
                new CreateProductProduceDto(Guid.NewGuid(), faker.Commerce.ProductName()))
            .Generate();
    }
}