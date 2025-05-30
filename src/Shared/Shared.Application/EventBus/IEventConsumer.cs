namespace Shared.Application.EventBus;

/// <summary>
/// Defines a contract for subscribing event handlers to specific event types.
/// Implementations are responsible for managing event subscriptions and dispatching events to handlers.
/// </summary>
public interface IEventConsumer
{
    /// <summary>
    /// Subscribes the specified event handler to events of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The event type, must inherit from <see cref="EventBase"/>.</typeparam>
    /// <param name="handler">The event handler instance that will process received events.</param>
    /// <param name="subscriberId">A unique identifier for the subscriber (used for queue or subscription naming).</param>
    /// <returns>A task representing the asynchronous subscription operation.</returns>
    Task SubscribeAsync<T>(IEventHandler<T> handler, string subscriberId) where T : EventBase;
}