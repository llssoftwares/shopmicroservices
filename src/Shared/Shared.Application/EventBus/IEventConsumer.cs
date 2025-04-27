namespace Shared.Application.EventBus;

public interface IEventConsumer
{
    Task SubscribeAsync<T>(IEventHandler<T> handler, string subscriberId) where T : EventBase;
}