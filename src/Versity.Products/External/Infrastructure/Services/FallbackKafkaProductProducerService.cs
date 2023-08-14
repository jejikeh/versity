using Application.Abstractions;
using Application.Dtos;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class FallbackKafkaProductProducerService : IProductProducerService
{
    private readonly ILogger<FallbackKafkaProductProducerService> _logger;

    public FallbackKafkaProductProducerService(ILogger<FallbackKafkaProductProducerService> logger)
    {
        _logger = logger;
    }

    public Task CreateProductProduce(CreateProductProduceDto createProductProduceDto, CancellationToken cancellationToken)
    {
        _logger.LogError("Kafka was not configured correctly! Fallback to FallbackKafkaProductProducerService. Please, check the kafka producer configuration");
        return Task.CompletedTask;
    }

    public Task DeleteProductProduce(DeleteProductDto deleteProductDto, CancellationToken cancellationToken)
    {
        _logger.LogError("Kafka was not configured correctly! Fallback to FallbackKafkaProductProducerService. Please, check the kafka producer configuration");
        return Task.CompletedTask;
    }
}