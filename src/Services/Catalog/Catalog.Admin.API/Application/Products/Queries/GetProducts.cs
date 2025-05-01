namespace Catalog.Admin.API.Application.Products.Queries;

public record GetProducts() : IQuery<GetProductsResponse>
{
    public IEnumerable<Guid> CategoriesIds { get; init; } = [];
}

public record GetProductsResponse(IEnumerable<ProductDto> Products);

public class GetProductsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/products", async (ISender sender, [FromQuery] Guid[] categoriesIds) =>
        {
            var response = await sender.Send(new GetProducts() { CategoriesIds = categoriesIds });
            return Results.Ok(response);
        })
        .WithName("GetProducts");
    }
}

public class GetProductsHandler(CatalogAdminDbContext dbContext)
    : IQueryHandler<GetProducts, GetProductsResponse>
{
    public async ValueTask<GetProductsResponse> Handle(GetProducts query, CancellationToken cancellationToken)
    {
        var list = await dbContext
            .Products
            .Where(x => !query.CategoriesIds.Any() || x.ProductCategories.Any(pc => query.CategoriesIds.Contains(pc.CategoryId)))
            .Select(x => new ProductDto(x.Id, x.Name, x.Price, x.ProductCategories.Select(pc => pc.CategoryId)))
            .ToListAsync(cancellationToken);

        return new(list);
    }
}