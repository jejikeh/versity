using System.Text.Json;
using Application.RequestHandlers.Products.Commands.CreateProduct;
using Application.RequestHandlers.Products.Commands.DeleteProduct;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AutoOffsetReset = Confluent.Kafka.AutoOffsetReset;

namespace Infrastructure.KafkaConsumerService;

public class KafkaProductConsumerService : IHostedService, IDisposable
{
    private readonly ILogger<KafkaProductConsumerService> _logger;
    private IConsumer<string, string> _consumer;
    private readonly ISender _sender;

    public KafkaProductConsumerService(ILogger<KafkaProductConsumerService> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;

        var config = new ConsumerConfig()
        {
            BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_Host"),
            GroupId = "versity.sessions",
            SecurityProtocol = SecurityProtocol.Plaintext,
            EnableAutoCommit = false,
            StatisticsIntervalMs = 5000,
            SessionTimeoutMs = 6000,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnablePartitionEof = true,
        };
        
        _consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => _logger.LogError($"Kafka Exception: ErrorCode:[{e.Code}] Reason:[{e.Reason}] Message:[{e.ToString()}]"))
            .Build();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Kafka Consumer Service has started.");

            _consumer.Subscribe(new List<string>() { Environment.GetEnvironmentVariable("KAFKA_Topic") });

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

                if (consumeResult.Topic.Equals(Environment.GetEnvironmentVariable("KAFKA_Topic")))
                {
                    await Task.Run(async () =>
                    {
                        var json = consumeResult.Message.Value;
                        _logger.LogInformation($"[{consumeResult.Message.Key}] {consumeResult.Topic} - {json}");

                        switch (consumeResult.Message.Key)
                        {
                            case "CreateProduct":
                            {
                                var command = JsonSerializer.Deserialize<CreateProductCommand>(consumeResult.Message.Value);
                                await _sender.Send(command, cancellationToken);
                                break;
                            }
                            case "DeleteProduct":
                            {
                                var command = JsonSerializer.Deserialize<DeleteProductCommand>(consumeResult.Message.Value);
                                await _sender.Send(command!, cancellationToken);
                                break;
                            }
                        }
                    }, 
                        cancellationToken).ConfigureAwait(false);
                }
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