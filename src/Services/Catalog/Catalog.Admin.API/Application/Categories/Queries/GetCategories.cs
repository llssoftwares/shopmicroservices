namespace Catalog.Admin.API.Application.Categories.Queries;

public record GetCategories();

public record GetCategoriesResponse(IEnumerable<CategoryDto> Products);

public class GetCategoriesQueryHandler(CatalogAdminDbContext dbContext)
{
    public async Task<GetCategoriesResponse> HandleAsync(GetCategories _)
    {
        var list = await dbContext
            .Categories.Select(x => new CategoryDto(x.Id, x.Name, x.Description))
            .ToListAsync();

        return new(list);
    }
}