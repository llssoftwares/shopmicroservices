namespace Catalog.Admin.API.Application.Categories.Queries;

public record GetCategories() : IQuery<GetCategoriesResponse>;

public record GetCategoriesResponse(IEnumerable<CategoryDto> Categories);

public class GetCategoriesHandler(CatalogAdminDbContext dbContext)
    : IQueryHandler<GetCategories, GetCategoriesResponse>
{
    public async ValueTask<GetCategoriesResponse> Handle(GetCategories query, CancellationToken cancellationToken)
    {
        var list = await dbContext
            .Categories.Select(x => new CategoryDto(x.Id, x.Name, x.Description))
            .ToListAsync(cancellationToken);

        return new(list);
    }
}