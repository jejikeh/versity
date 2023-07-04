using Application.Abstractions;
using Application.Dtos;
using KafkaFlow.Producers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class ProductProducerService : IProductProducerService
{
    private readonly IProducerAccessor _producerAccessor;
    private readonly ILogger<ProductProducerService> _logger;

    public ProductProducerService(IProducerAccessor producerAccessor, ILogger<ProductProducerService> logger)
    {
        _producerAccessor = producerAccessor;
        _logger = logger;
    }

    public async Task CreateProductProduce(CreateProductProduceDto createProductProduceDto, CancellationToken cancellationToken)
    {
        try 
        {
            var producer = _producerAccessor.GetProducer(Environment.GetEnvironmentVariable("KAFKA_ProducerName"));

            await producer.ProduceAsync("CreateProduct", createProductProduceDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Host terminated unexpectedly");
        }

    }

    public async Task DeleteProductProduce(DeleteProductDto deleteProductDto, CancellationToken cancellationToken)
    {
        try 
        {
            var producer = _producerAccessor.GetProducer(Environment.GetEnvironmentVariable("KAFKA_ProducerName"));

            await producer.ProduceAsync("DeleteProduct", deleteProductDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Host terminated unexpectedly");
        }
    }
}