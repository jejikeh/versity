using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.KafkaConsumer;

public class FallbackKafkaProductConsumerService : BackgroundService
{
    private readonly ILogger<FallbackKafkaProductConsumerService> _logger;

    public FallbackKafkaProductConsumerService(ILogger<FallbackKafkaProductConsumerService> logger)
    {
        _logger = logger;

        _logger.LogError("Kafka Consumer was not configured correctly! Fallback to FallbackKafkaProductConsumerService. Please, check the kafka producer configuration");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("--> Kafka Consumer Service is running.");
        return Task.CompletedTask;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogError("Kafka Consumer was not configured correctly! Fallback to FallbackKafkaProductConsumerService. Please, check the kafka producer configuration");
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogError("Kafka Consumer was not configured correctly! Fallback to FallbackKafkaProductConsumerService. Please, check the kafka producer configuration");
        return Task.CompletedTask;
    }
}