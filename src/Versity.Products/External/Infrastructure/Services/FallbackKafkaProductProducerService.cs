using System.Net.Http.Json;
using Application.Abstractions;
using Application.Dtos;
using Infrastructure.Configurations;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class FallbackKafkaProductProducerService : IProductProducerService
{
    private readonly ILogger<FallbackKafkaProductProducerService> _logger;
    private readonly HttpClient _httpClient;

    public FallbackKafkaProductProducerService(
        ILogger<FallbackKafkaProductProducerService> logger,
        IApplicationConfiguration applicationConfiguration)
    {
        _logger = logger;
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(applicationConfiguration.DevelopmentSessionServiceHost)
        };
    }

    public async Task CreateProductProduce(CreateProductProduceDto createProductProduceDto, CancellationToken cancellationToken)
    {
        _logger.LogError("Kafka was not configured correctly! Fallback to FallbackKafkaProductProducerService. Please, check the kafka producer configuration");
        await _httpClient.PostAsJsonAsync("api/internalproducts/", createProductProduceDto, cancellationToken: cancellationToken);
    }

    public async Task DeleteProductProduce(DeleteProductDto deleteProductDto, CancellationToken cancellationToken)
    {
        _logger.LogError("Kafka was not configured correctly! Fallback to FallbackKafkaProductProducerService. Please, check the kafka producer configuration");
        await _httpClient.DeleteAsync("api/internalproducts/" + deleteProductDto.Id, cancellationToken);
    }
}