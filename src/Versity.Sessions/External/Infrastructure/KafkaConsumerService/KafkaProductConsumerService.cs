using Confluent.Kafka;
using Infrastructure.KafkaConsumerService.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.KafkaConsumerService;

public class KafkaProductConsumerService : IHostedService, IDisposable
{
    private readonly ILogger<KafkaProductConsumerService> _logger;
    private readonly IConsumer<string, string> _consumer;
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

        _consumer = new ConsumerBuilder<string, string>(_configuration.Config)
            .SetErrorHandler((_, e) => _logger.LogError($"Kafka Exception: ErrorCode:[{e.Code}] Reason:[{e.Reason}] Message:[{e.ToString()}]"))
            .Build();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Kafka Consumer Service has started.");

            _consumer.Subscribe(new List<string>() { _configuration.Topic });

            await Consume(cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kafka Consumer Service is stopping.");

        _consumer.Close();

        await Task.CompletedTask;
    }
    
    private async Task Consume(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(cancellationToken);

                if (consumeResult?.Message == null)
                {
                    continue;
                }

                if (!consumeResult.Topic.Equals(_configuration.Topic))
                {
                    continue;
                }
                
                _logger.LogInformation($"[{consumeResult.Message.Key}] {consumeResult.Topic} - {consumeResult.Message.Value}");
                    
                using var scope = _serviceProvider.CreateScope();
                var kafkaHandlersContainer = scope.ServiceProvider.GetRequiredService<IKafkaHandlersContainer>();
                    
                await kafkaHandlersContainer.ProcessMessage(
                    consumeResult.Message.Key, 
                    consumeResult.Message.Value,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
    
    public void Dispose()
    {
        _consumer.Dispose();
    }
}