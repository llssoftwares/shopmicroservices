namespace Shared.Application.EventBus;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event) where T : EventBase;
}