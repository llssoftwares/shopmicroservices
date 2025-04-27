using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Shared.Application.EventBus;

public class EventBusRabbitMQ : IEventPublisher, IEventConsumer
{
    private readonly ConnectionFactory _factory;

    public EventBusRabbitMQ(string hostName)
    {
        _factory = new ConnectionFactory() { HostName = hostName };
    }

    public async Task PublishAsync<T>(T @event) where T : EventBase
    {
        using var connection = await _factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: "events", type: "fanout");

        var json = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(exchange: "events", routingKey: "", body: body);
    }

    public async Task SubscribeAsync<T>(IEventHandler<T> handler, string subscriberId) where T : EventBase
    {
        await Task.Run(async () =>
        {
            var connection = await _factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchange: "events", type: "fanout");

            var queueName = $"{typeof(T).Name}.{subscriberId}";
            await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync(queue: queueName, exchange: "events", routingKey: "");

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonSerializer.Deserialize<T>(json);

                if (message != null)
                {
                    await handler.HandleAsync(message);
                }
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
        });
    }
}