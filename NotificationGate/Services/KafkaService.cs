using System.Text.Json;
using Confluent.Kafka;
using NotificationGate.Models;

namespace NotificationGate.Services;

public class KafkaService: IDisposable
{
    private readonly ILogger<KafkaService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IProducer<string, string> _producer;

    public KafkaService(ILogger<KafkaService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        var config = new ProducerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServer"]
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public void Send(string topic,Alert alert)
    {
        
        var message = new Message<string, string>
        {
            Key = alert.Source,
            Value = JsonSerializer.Serialize(alert)
        };
        try
        {
            _producer.Produce(topic, message);
        }
        catch (ProduceException<string, string>)
        {
            _logger.LogError("Alert {alertId} Produce failed", alert.AlertId);
        }
        catch (Exception e)
        {
            _logger.LogError("Error {e} when produce", e);
        }
        _logger.LogInformation("Alert {alertId} Produce successfully", alert.AlertId);
    }
    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }
    
}