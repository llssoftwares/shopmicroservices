namespace Shared.Application.EventBus;

/// <summary>
/// Defines a contract for handling events of a specific type.
/// Implementations should provide logic to process events when they are received.
/// </summary>
/// <typeparam name="TEvent">The type of event to handle.</typeparam>
public interface IEventHandler<TEvent>
{
    /// <summary>
    /// Handles the specified event asynchronously.
    /// </summary>
    /// <param name="event">The event instance to process.</param>
    /// <returns>A task representing the asynchronous handling operation.</returns>
    Task HandleAsync(TEvent @event);
}