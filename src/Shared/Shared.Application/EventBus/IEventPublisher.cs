namespace Shared.Application.EventBus;

/// <summary>
/// Defines a contract for publishing events of a specific type.
/// Implementations are responsible for delivering events to the appropriate transport or bus.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes the specified event asynchronously.
    /// </summary>
    /// <typeparam name="T">The event type, must inherit from <see cref="EventBase"/>.</typeparam>
    /// <param name="event">The event instance to publish.</param>
    /// <returns>A task representing the asynchronous publish operation.</returns>
    Task PublishAsync<T>(T @event) where T : EventBase;
}