namespace Catalog.Admin.API.Domain;

public class Category
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public ICollection<ProductCategory> ProductCategories { get; init; } = [];
}