using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.EventBus;
using Shared.Application.Exceptions.Handler;
using System.Reflection;

namespace Shared.Application;

/// <summary>
/// Provides extension methods for registering and configuring application services and middleware.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the custom exception handler for the application.
    /// </summary>
    /// <param name="services">The service collection to add the handler to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCustomExceptionHandler(this IServiceCollection services)
    {
        // Registers CustomExceptionHandler as the application's exception handler.
        return services.AddExceptionHandler<CustomExceptionHandler>();
    }

    /// <summary>
    /// Configures the application to use the custom exception handler middleware.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The updated application builder.</returns>
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        // Adds the exception handler middleware to the request pipeline.
        return app.UseExceptionHandler(options => { });
    }

    /// <summary>
    /// Registers the RabbitMQ event bus as the implementation for IEventPublisher and IEventConsumer.
    /// </summary>
    /// <param name="services">The service collection to add the event bus to.</param>
    /// <param name="configuration">The application configuration (used to get the RabbitMQ connection string).</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        // Creates a single EventBusRabbitMQ instance and registers it for both publishing and consuming events.
        var eventBus = new EventBusRabbitMQ(configuration.GetConnectionString("RabbitMQ")!);

        return services
            .AddTransient<IEventPublisher>(x => eventBus)
            .AddTransient<IEventConsumer>(x => eventBus);
    }

    /// <summary>
    /// Registers all event handler implementations found in the specified assemblies.
    /// If no assemblies are specified, scans all loaded assemblies in the current AppDomain.
    /// </summary>
    /// <param name="services">The service collection to add the handlers to.</param>
    /// <param name="assemblies">Assemblies to scan for event handlers.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddEventHandlers(this IServiceCollection services, params Assembly[] assemblies)
    {
        // If no assemblies are provided, scan all non-dynamic loaded assemblies.
        if (assemblies == null || assemblies.Length == 0)
        {
            assemblies = [.. AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.FullName))];
        }

        // Registers all classes implementing IEventHandler<T> as scoped services.
        return services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }

    /// <summary>
    /// Subscribes all registered event handlers to the event bus at application startup.
    /// </summary>
    /// <param name="app">The web application instance.</param>
    /// <param name="subscriberId">
    /// Optional subscriber identifier. If not provided, uses the entry assembly name or a default value.
    /// </param>
    public static async Task UseEventHandlers(this WebApplication app, string? subscriberId = null)
    {
        // Determine the subscriber ID if not provided.
        if (string.IsNullOrEmpty(subscriberId))
        {
            var entryAssembly = Assembly.GetEntryAssembly();

            subscriberId = entryAssembly?.GetName().Name
                ?? "DefaultSubscriberId";
        }

        // Create a service scope to resolve event consumer and handlers.
        using var scope = app.Services.CreateScope();

        var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();

        // Automatically subscribe all event handlers using the resolved event consumer.
        await eventConsumer.AutoSubscribeAllHandlers(scope.ServiceProvider, subscriberId);
    }
}