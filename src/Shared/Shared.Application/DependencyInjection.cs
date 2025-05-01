using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Behaviors;
using Shared.Application.EventBus;
using Shared.Application.Exceptions.Handler;
using System.Reflection;

namespace Shared.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomExceptionHandler(this IServiceCollection services)
    {
        return services.AddExceptionHandler<CustomExceptionHandler>();
    }

    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseExceptionHandler(options => { });
    }

    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        var eventBus = new EventBusRabbitMQ(configuration.GetConnectionString("RabbitMQ")!);

        return services
            .AddTransient<IEventPublisher>(x => eventBus)
            .AddTransient<IEventConsumer>(x => eventBus);
    }

    public static IServiceCollection AddEventHandlers(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
        {
            assemblies = [.. AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.FullName))];
        }

        return services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }

    public static async Task UseEventHandlers(this WebApplication app, string? subscriberId = null)
    {
        if (string.IsNullOrEmpty(subscriberId))
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            subscriberId = entryAssembly?.GetName().Name
                ?? "DefaultSubscriberId";
        }

        using var scope = app.Services.CreateScope();

        var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();

        await eventConsumer.AutoSubscribeAllHandlers(scope.ServiceProvider, subscriberId);
    }
}