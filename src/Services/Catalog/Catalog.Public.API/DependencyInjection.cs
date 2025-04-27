namespace Catalog.Public.API;

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
        services.AddTransient<GetProductsHandler>();
        services.AddTransient<ProductCreatedHandler>();
        services.AddTransient<ProductRepository>();

        return services;
    }    
}
