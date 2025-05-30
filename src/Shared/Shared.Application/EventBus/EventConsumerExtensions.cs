namespace Shared.Application.EventBus;

/// <summary>
/// Extension methods for IEventConsumer to simplify event handler subscription.
/// </summary>
public static class EventConsumerExtensions
{
    /// <summary>
    /// Automatically discovers and subscribes all registered IEventHandler&lt;T&gt; implementations
    /// in the current AppDomain to the event consumer, using reflection and dependency injection.
    /// </summary>
    /// <param name="eventConsumer">The event consumer instance.</param>
    /// <param name="serviceProvider">The service provider for resolving handler instances.</param>
    /// <param name="subscriberId">A unique identifier for the subscriber (used in queue naming).</param>
    public static async Task AutoSubscribeAllHandlers(this IEventConsumer eventConsumer, IServiceProvider serviceProvider, string subscriberId)
    {
        // Discover all non-abstract, non-interface types that implement IEventHandler<T>
        var handlerTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic)
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                .Select(i => new { Handler = t, EventType = i.GetGenericArguments()[0] }))
            .ToList();

        // For each discovered handler, resolve the instance and subscribe it to the event consumer
        foreach (var ht in handlerTypes)
        {
            // Get the generic SubscribeAsync<T> method for the event type
            var method = typeof(IEventConsumer)
                .GetMethod(nameof(IEventConsumer.SubscribeAsync))?
                .MakeGenericMethod(ht.EventType);

            if (method != null)
            {
                // Resolve the handler instance from the service provider
                var handlerType = typeof(IEventHandler<>).MakeGenericType(ht.EventType);
                var handler = serviceProvider.GetService(handlerType)
                    ?? throw new InvalidOperationException($"Handler for {ht.EventType.Name} not registered.");

                // Invoke SubscribeAsync<T> with the resolved handler and subscriberId
                await (Task)method.Invoke(eventConsumer, [handler, subscriberId])!;
            }
        }
    }
}
