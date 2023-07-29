using Application.Abstractions;
using Application.Dtos;
using Infrastructure.Configurations;
using KafkaFlow.Producers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class KafkaProductProducerService : IProductProducerService
{
    private readonly IProducerAccessor _producerAccessor;
    private readonly ILogger<KafkaProductProducerService> _logger;
    private readonly IKafkaProducerConfiguration _kafkaProducerConfiguration;

    public KafkaProductProducerService(IProducerAccessor producerAccessor, ILogger<KafkaProductProducerService> logger, IKafkaProducerConfiguration kafkaProducerConfiguration)
    {
        _producerAccessor = producerAccessor;
        _logger = logger;
        _kafkaProducerConfiguration = kafkaProducerConfiguration;
    }

    public async Task CreateProductProduce(CreateProductProduceDto createProductProduceDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Send message CreateProductProduce to the kafka!");
        
        var producer = _producerAccessor.GetProducer(_kafkaProducerConfiguration.ProducerName);

        await producer.ProduceAsync(_kafkaProducerConfiguration.CreateProductTopic, createProductProduceDto);
    }

    public async Task DeleteProductProduce(DeleteProductDto deleteProductDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Send message DeleteProductProduce to the kafka!");

        var producer = _producerAccessor.GetProducer(_kafkaProducerConfiguration.ProducerName);

        await producer.ProduceAsync(_kafkaProducerConfiguration.DeleteProductTopic, deleteProductDto);
    }
}