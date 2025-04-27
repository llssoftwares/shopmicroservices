namespace Shared.Application.EventBus;

public abstract class EventBase
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}