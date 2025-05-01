namespace Cart.API;

public static class DependencyInjection
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = configuration.GetConnectionString("RedisInstance");
        });

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services
            .AddMediator(options => { options.ServiceLifetime = ServiceLifetime.Scoped; })
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandLoggingBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(QueryLoggingBehavior<,>))
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())            
            .AddTransient<ProductRepository>()
            .AddTransient<ShoppingCartRepository>();
    }
}
