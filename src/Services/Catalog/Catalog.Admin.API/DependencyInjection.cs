namespace Catalog.Admin.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<CreateProductCommandHandler>();
        services.AddTransient<GetProductsQueryHandler>();
        services.AddTransient<CreateCategoryCommandHandler>();
        services.AddTransient<GetCategoriesQueryHandler>();

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogAdminDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("Database")));

        return services;
    }    
}
