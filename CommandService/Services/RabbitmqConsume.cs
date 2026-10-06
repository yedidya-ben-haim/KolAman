using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;

namespace CommandService.Services;

public class RabbitmqConsume
{
    private readonly IConfiguration _configuration;
    private readonly Task<IConnection> _connection;

    public RabbitmqConsume(IConfiguration configuration)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        var _connection = factory.CreateConnectionAsync();
    }
    
    public async Task ReceiveMessage()
    {
        var connection = await _connection;
        using (var channel = await connection.CreateChannelAsync())
        {
            await channel.QueueDeclareAsync(queue: "NORTH", durable: true, exclusive: false, autoDelete: false);
            
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);
                return Task.CompletedTask;
            };
            await channel.BasicConsumeAsync("NORTH", autoAck: true, consumer: consumer);
        }
    }
    
}