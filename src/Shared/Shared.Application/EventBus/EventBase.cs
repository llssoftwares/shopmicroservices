namespace Shared.Application.EventBus;

/// <summary>
/// Serves as the base class for all event types in the event bus system.
/// Provides a common <see cref="Timestamp"/> property to record when the event was created.
/// </summary>
public abstract class EventBase
{
    /// <summary>
    /// Gets or sets the UTC timestamp indicating when the event was created.
    /// Defaults to the current UTC time upon instantiation.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}