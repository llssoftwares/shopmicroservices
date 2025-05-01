namespace Catalog.Admin.API.Application.Categories.Queries;

public record GetCategories() : IQuery<GetCategoriesResponse>;

public record GetCategoriesResponse(IEnumerable<CategoryDto> Categories);

public class GetCategoriesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/categories", async (ISender sender) =>
        {
            var response = await sender.Send(new GetCategories());
            return Results.Ok(response);
        })
        .WithName("GetCategories");
    }
}

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