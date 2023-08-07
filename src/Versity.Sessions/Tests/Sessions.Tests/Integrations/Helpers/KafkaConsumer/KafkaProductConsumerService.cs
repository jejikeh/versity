using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Products.Tests.Integrations.Helpers.KafkaConsumer.Abstractions;

namespace Sessions.Tests.Integrations.Helpers.KafkaConsumer;

public class KafkaProductConsumerService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly IKafkaConsumerConfiguration _configuration;

    public KafkaProductConsumerService(
        ILogger<KafkaProductConsumerService> logger, 
        IKafkaConsumerConfiguration configuration)
    {
        _configuration = configuration;

        _consumer = new ConsumerBuilder<string, string>(_configuration.Config)
            .Build();
    }

    public ConsumeResult<string, string>? Consume()
    {
        try
        {
            var consumeResult = _consumer.Consume();
            
            if (consumeResult?.Message == null || !consumeResult.Topic.Equals(_configuration.Topic))
            {
                return null;
            }

            return consumeResult;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public void Start()
    {
        _consumer.Subscribe(new List<string>() { _configuration.Topic });
    }

    public void Stop()
    {
        _consumer.Close();
    }
}