using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Codemy.BuildingBlocks.EventBus.Interfaces;

namespace Codemy.BuildingBlocks.EventBus.RabbitMQ
{
    public class RabbitMqPublisher : IEventPublisher
    {

        public RabbitMqPublisher()
        {
        }

        public void Publish<T>(T @event, string queueName)
        {
            var factory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
                Port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672"),
                UserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME") ?? "guest",
                Password = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest"
            };

            using var connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            using var channel = connection.CreateChannelAsync().GetAwaiter().GetResult();

            channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
            channel.BasicPublishAsync(exchange: "", routingKey: queueName, body: body).GetAwaiter().GetResult();
        }
    }
}
