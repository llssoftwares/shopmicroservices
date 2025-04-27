namespace Catalog.Public.API.Domain;

public class Product
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public decimal Price { get; init; }

    public ICollection<Category> Categories { get; init; } = [];
}