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
        services.AddTransient<GetCartQueryHandler>();
        services.AddTransient<CreateCartCommandHandler>();
        services.AddTransient<AddItemCommandHandler>();
        services.AddTransient<ShoppingCartRepository>();
        services.AddTransient<ProductRepository>();
        services.AddTransient<ProductCreatedHandler>();        

        return services;
    }
}
