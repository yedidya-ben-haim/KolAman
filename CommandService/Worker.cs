using CommandService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using CommandService.Models;
using Microsoft.EntityFrameworkCore.Internal;


namespace CommandService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly AlertValidator _alertValidator;
    private readonly IServiceScopeFactory _scopeFactory;

    public Worker(ILogger<Worker> logger, RabbitmqConsume rabbitmqConsume, AlertValidator alertValidator, 
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _alertValidator = alertValidator;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var command = Environment.GetEnvironmentVariable("COMMAND") ?? "NORTH";

        using var scope = _scopeFactory.CreateScope();
            
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: command, durable: true, exclusive: false, autoDelete: false,
            arguments: new Dictionary<string, object?> { { "x-queue-type", "quorum" } });

        _logger.LogInformation("Waiting for messages.");
        
        while (!stoppingToken.IsCancellationRequested)
        {
             
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Message received, command: [{command}]", ea.RoutingKey);

                try
                {
                    var alert = JsonSerializer.Deserialize<Alert>(message);

                    if (!_alertValidator.ValidateAlert(alert))
                    {
                        _logger.LogError("alert {alertId} not validate", alert.AlertId);
                    }

                    var alertEntity = new AlertEntity
                    {
                        AlertId = alert.AlertId,
                        Source = alert.Source,
                        Title = alert.Title,
                        Content = alert.Content,
                        Priority = alert.Priority,
                        Classification = alert.Classification,
                        Lat = decimal.Parse(alert.Lat),
                        Lon = decimal.Parse(alert.Lon),
                        Timestamp = DateTime.Parse(alert.Timestamp),
                        Status = alert.Status
                    };
                    dbContext.Alerts.Add(alertEntity);
                    await dbContext.SaveChangesAsync();
                }
                catch (JsonException e)
                {
                    _logger.LogError("error deserialize rabbit message");
                }
                
                await Task.CompletedTask;
            };
            await channel.BasicConsumeAsync(command, autoAck: true, consumer: consumer,
                cancellationToken: stoppingToken);
            
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}