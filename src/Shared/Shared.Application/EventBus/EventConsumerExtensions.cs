namespace Shared.Application.EventBus;

public static class EventConsumerExtensions
{
    public static async Task AutoSubscribeAllHandlers(this IEventConsumer eventConsumer, IServiceProvider serviceProvider, string subscriberId)
    {
        var handlerTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic)
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                .Select(i => new { Handler = t, EventType = i.GetGenericArguments()[0] }))
            .ToList();

        foreach (var ht in handlerTypes)
        {
            var method = typeof(IEventConsumer)
                .GetMethod(nameof(IEventConsumer.SubscribeAsync))?
                .MakeGenericMethod(ht.EventType);

            if (method != null)
            {
                var handlerType = typeof(IEventHandler<>).MakeGenericType(ht.EventType);
                var handler = serviceProvider.GetService(handlerType)
                    ?? throw new InvalidOperationException($"Handler for {ht.EventType.Name} not registered.");

                await (Task)method.Invoke(eventConsumer, [handler, subscriberId])!;
            }
        }
    }
}
