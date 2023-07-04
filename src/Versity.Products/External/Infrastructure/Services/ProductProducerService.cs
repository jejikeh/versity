using Application.Abstractions;
using Application.Dtos;
using KafkaFlow.Producers;

namespace Infrastructure.Services;

public class ProductProducerService : IProductProducerService
{
    private readonly IProducerAccessor _producerAccessor;

    public ProductProducerService(IProducerAccessor producerAccessor)
    {
        _producerAccessor = producerAccessor;
    }

    public async Task CreateProductProduce(CreateProductProduceDto createProductProduceDto, CancellationToken cancellationToken)
    {
        var producer = _producerAccessor.GetProducer(Environment.GetEnvironmentVariable("KAFKA_ProducerName"));

        await producer.ProduceAsync("CreateProduct", createProductProduceDto);
    }

    public async Task DeleteProductProduce(DeleteProductDto deleteProductDto, CancellationToken cancellationToken)
    {
        var producer = _producerAccessor.GetProducer(Environment.GetEnvironmentVariable("KAFKA_ProducerName"));
        
        await producer.ProduceAsync("DeleteProduct", deleteProductDto);
    }
}