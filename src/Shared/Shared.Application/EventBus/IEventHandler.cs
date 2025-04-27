namespace Shared.Application.EventBus;

public interface IEventHandler<TEvent>
{
    Task HandleAsync(TEvent @event);
}