using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.EventBus;
using Shared.Application.Exceptions.Handler;
using System.Reflection;

namespace Shared.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<CustomExceptionHandler>();

        return services;
    }

    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(options => { });

        return app;
    }

    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        var eventBus = new EventBusRabbitMQ(configuration.GetConnectionString("RabbitMQ")!);

        services.AddTransient<IEventPublisher>(x => eventBus);
        services.AddTransient<IEventConsumer>(x => eventBus);

        return services;
    }

    public static IServiceCollection AddEventHandlers(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
        {
            assemblies = [.. AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.FullName))];
        }

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    public static async Task UseEventHandlers(this WebApplication app, string? subscriberId = null)
    {
        if (string.IsNullOrEmpty(subscriberId))
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            var mainNamespace = entryAssembly?.GetTypes()
                .FirstOrDefault(type => type.Namespace != null)?
                .Namespace;

            subscriberId = mainNamespace ?? "DefaultSubscriberId";
        }

        using var scope = app.Services.CreateScope();

        var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();

        await eventConsumer.AutoSubscribeAllHandlers(scope.ServiceProvider, subscriberId);
    }
}
