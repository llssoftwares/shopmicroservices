namespace Catalog.Admin.API.Application.Products.Commands;

public record CreateProduct(string Name, decimal Price) : ICommand<CreateProductResponse>
{
    public IEnumerable<Guid> CategoriesIds { get; init; } = [];
};

public record CreateProductResponse(Guid ProductId);

public class CreateProductValidator : AbstractValidator<CreateProduct>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
    }
}

public class CreateProductHandler(CatalogAdminDbContext dbContext, IEventPublisher eventPublisher)
    : ICommandHandler<CreateProduct, CreateProductResponse>
{
    public async ValueTask<CreateProductResponse> Handle(CreateProduct command, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Price = command.Price,
            ProductCategories = [.. command.CategoriesIds.Select(x => new ProductCategory { CategoryId = x })]
        };

        await dbContext.Products.AddAsync(product, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        var categoriesNames = await dbContext
            .Categories
            .Where(x => command.CategoriesIds.Contains(x.Id))
            .Select(x => new { x.Id, x.Name })
            .ToDictionaryAsync(x => x.Id, x => x.Name, cancellationToken);

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