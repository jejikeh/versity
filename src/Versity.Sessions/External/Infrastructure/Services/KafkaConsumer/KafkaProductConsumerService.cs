using Confluent.Kafka;
using Infrastructure.Services.KafkaConsumer.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.KafkaConsumer;

public class KafkaProductConsumerService : BackgroundService
{
    private readonly ILogger<KafkaProductConsumerService> _logger;
    private readonly IConsumer<string, string>? _consumer;
    private readonly IKafkaConsumerConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public KafkaProductConsumerService(
        ILogger<KafkaProductConsumerService> logger, 
        IKafkaConsumerConfiguration configuration, 
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;

        if (Environment.GetEnvironmentVariable("KAFKA_Host") != "no_set")
        {
            _consumer = new ConsumerBuilder<string, string>(_configuration.Config)
                .SetErrorHandler((_, e) => _logger.LogError($"Kafka Exception: " +
                                                            $"ErrorCode:[{e.Code}] " +
                                                            $"Reason:[{e.Reason}] " +
                                                            $"Message:[{e}]"))
                .Build();
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("--> Kafka Consumer Service is running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (ConsumeFromTopic(out var consumeResult))
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    continue;
                }

                _logger.LogInformation(
                    $"[{consumeResult.Message.Key}] {consumeResult.Topic} - {consumeResult.Message.Value}");

                using var scope = _serviceProvider.CreateScope();
                var kafkaHandlersContainer = scope.ServiceProvider.GetRequiredService<IKafkaHandlersContainer>();

                await kafkaHandlersContainer.ProcessMessage(
                    consumeResult.Message.Key,
                    consumeResult.Message.Value,
                    stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"--> Kafka Consume error: {ex.Message}" +
                                 $"\n STACK: {ex.StackTrace} " +
                                 $"\n SOURCE: {ex.Source}");
            }
        }
    }

    private bool ConsumeFromTopic(out ConsumeResult<string, string>? consumeResult)
    {
        if (_consumer != null && !_consumer.Assignment.Exists(topicPartition =>
                string.Equals(topicPartition.Topic, _configuration.Topic)))
        {
            _logger.LogInformation($"The topic {_configuration.Topic} is not available");
            consumeResult = null;
            
            return true;
        }

        consumeResult = _consumer?.Consume();

        return consumeResult?.Message == null || !consumeResult.Topic.Equals(_configuration.Topic);
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("--> Kafka Consumer Service has started.");

        if (_consumer is null)
        {
            return;
        }

        _consumer.Subscribe(new List<string>() { _configuration.Topic });
        
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("--> Kafka Consumer Service is stopping.");

        if (_consumer is null)
        {
            return;
        }

        _consumer.Close();

        await base.StopAsync(cancellationToken);
    }
}