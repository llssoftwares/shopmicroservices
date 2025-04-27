namespace Catalog.Admin.API.Domain;

public class ProductCategory
{
    public Guid ProductId { get; init; }

    public Product Product { get; init; } = default!;

    public Guid CategoryId { get; init; }

    public Category Category { get; init; } = default!;
}