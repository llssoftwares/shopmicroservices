namespace Catalog.Admin.API.Application.Products.Queries;

public record GetProducts()
{
    public IEnumerable<Guid> CategoriesIds { get; init; } = [];
}

public record GetProductsResponse(IEnumerable<ProductDto> Products);

public class GetProductsQueryHandler(CatalogAdminDbContext dbContext)
{
    public async Task<GetProductsResponse> HandleAsync(GetProducts query)
    {
        var list = await dbContext
            .Products
            .Where(x => !query.CategoriesIds.Any() || x.ProductCategories.Any(pc => query.CategoriesIds.Contains(pc.CategoryId)))
            .Select(x => new ProductDto(x.Id, x.Name, x.Price, x.ProductCategories.Select(pc => pc.CategoryId)))
            .ToListAsync();

        return new(list);
    }
}