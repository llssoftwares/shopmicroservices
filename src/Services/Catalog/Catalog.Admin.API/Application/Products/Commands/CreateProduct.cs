namespace Catalog.Admin.API.Application.Products.Commands;

public record CreateProduct(string Name, decimal Price)
{
    public IEnumerable<Guid> CategoriesIds { get; init; } = [];
};

public record CreateProductResponse(Guid ProductId);

public class CreateProductCommandHandler(CatalogAdminDbContext dbContext, IEventPublisher eventPublisher)
{
    public async Task<CreateProductResponse> HandleAsync(CreateProduct command)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Price = command.Price,
            ProductCategories = [.. command.CategoriesIds.Select(x => new ProductCategory { CategoryId = x })]
        };

        await dbContext.Products.AddAsync(product);

        await dbContext.SaveChangesAsync();

        var categoriesNames = await dbContext
            .Categories
            .Where(x => command.CategoriesIds.Contains(x.Id))
            .Select(x => new { x.Id, x.Name })
            .ToDictionaryAsync(x => x.Id, x => x.Name);

        await eventPublisher.PublishAsync(new ProductCreated
        {
            Id = product.Id,
            Name = command.Name,
            Price = command.Price,
            CategoriesNames = categoriesNames
        });

        return new(product.Id);
    }
}