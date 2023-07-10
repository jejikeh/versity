using Application.Abstractions;
using Application.Dtos;
using KafkaFlow.Producers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class KafkaProductProducerService : IProductProducerService
{
    private readonly IProducerAccessor _producerAccessor;
    private readonly ILogger<KafkaProductProducerService> _logger;

    public KafkaProductProducerService(IProducerAccessor producerAccessor, ILogger<KafkaProductProducerService> logger)
    {
        _producerAccessor = producerAccessor;
        _logger = logger;
    }

    public async Task CreateProductProduce(CreateProductProduceDto createProductProduceDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Send message CreateProductProduce to the kafka!");
        
        var producer = _producerAccessor.GetProducer(Environment.GetEnvironmentVariable("KAFKA_ProducerName"));

        await producer.ProduceAsync("CreateProduct", createProductProduceDto);
    }

    public async Task DeleteProductProduce(DeleteProductDto deleteProductDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Send message DeleteProductProduce to the kafka!");

        var producer = _producerAccessor.GetProducer(Environment.GetEnvironmentVariable("KAFKA_ProducerName"));

        await producer.ProduceAsync("DeleteProduct", deleteProductDto);
    }
}