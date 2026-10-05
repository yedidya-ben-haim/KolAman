using System.Text.Json;
using Confluent.Kafka;
using NotificationGate.Models;

namespace NotificationGate.Services;

public class KafkaService: IDisposable
{
    private readonly ILogger<KafkaService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IProducer<Null, string> _producer;

    public KafkaService(ILogger<KafkaService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        var config = new ProducerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServer"]
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public void Send(string topic,Alert alert)
    {
        
        var message = new Message<Null, string>
        {
            Value = JsonSerializer.Serialize(alert)
        };
        try
        {
            _producer.Produce(topic, message);
            _logger.LogInformation("Alert {alertId} Produce successfully", alert.AlertId);
        }
        catch (Exception e)
        {
            _logger.LogError("Error {e} when produce", e);
        }
    }
    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }
    
}