using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Shared.Application.EventBus;

/// <summary>
/// Implements event publishing and consuming using RabbitMQ as the transport.
/// </summary>
public class EventBusRabbitMQ : IEventPublisher, IEventConsumer
{
    private readonly ConnectionFactory _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventBusRabbitMQ"/> class with the specified RabbitMQ host.
    /// </summary>
    /// <param name="hostName">The RabbitMQ server host name.</param>
    public EventBusRabbitMQ(string hostName)
    {
        _factory = new ConnectionFactory() { HostName = hostName };
    }

    /// <summary>
    /// Publishes an event to the RabbitMQ "events" exchange using the "fanout" type.
    /// </summary>
    /// <typeparam name="T">The type of the event, must inherit from <see cref="EventBase"/>.</typeparam>
    /// <param name="event">The event instance to publish.</param>
    public async Task PublishAsync<T>(T @event) where T : EventBase
    {
        // Establish a connection and channel to RabbitMQ
        using var connection = await _factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // Declare a "fanout" exchange named "events"
        await channel.ExchangeDeclareAsync(exchange: "events", type: "fanout");

        // Serialize the event to JSON and publish it to the exchange
        var json = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(exchange: "events", routingKey: "", body: body);
    }

    /// <summary>
    /// Subscribes to events of type <typeparamref name="T"/> from the RabbitMQ "events" exchange.
    /// Each subscriber gets its own queue, identified by <paramref name="subscriberId"/>.
    /// </summary>
    /// <typeparam name="T">The type of the event, must inherit from <see cref="EventBase"/>.</typeparam>
    /// <param name="handler">The event handler to process received events.</param>
    /// <param name="subscriberId">A unique identifier for the subscriber (used in the queue name).</param>
    public async Task SubscribeAsync<T>(IEventHandler<T> handler, string subscriberId) where T : EventBase
    {
        // Run the subscription logic on a background thread
        await Task.Run(async () =>
        {
            // Establish a connection and channel to RabbitMQ
            var connection = await _factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            // Declare a "fanout" exchange named "events"
            await channel.ExchangeDeclareAsync(exchange: "events", type: "fanout");

            // Declare and bind a queue specific to the event type and subscriber
            var queueName = $"{typeof(T).Name}.{subscriberId}";
            await channel.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync(queue: queueName, exchange: "events", routingKey: "");

            // Create a consumer to process incoming messages
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                // Deserialize the message and invoke the handler
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonSerializer.Deserialize<T>(json);

                if (message != null)
                {
                    await handler.HandleAsync(message);
                }
            };

            // Start consuming messages from the queue
            await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
        });
    }
}