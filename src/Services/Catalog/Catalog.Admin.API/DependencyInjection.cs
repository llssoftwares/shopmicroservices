namespace Catalog.Admin.API;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogAdminDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("Database")));

        var serviceProvider = services.BuildServiceProvider();

        if (!bool.TryParse(configuration["Database:Migrate"], out var migrate) || !migrate)
            return services;

        using var dbContext = serviceProvider.GetRequiredService<CatalogAdminDbContext>();

        dbContext.Database.Migrate();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services
            .AddMediator(options => { options.ServiceLifetime = ServiceLifetime.Scoped; })
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }
}