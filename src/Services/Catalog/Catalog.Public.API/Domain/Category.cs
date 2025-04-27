namespace Catalog.Public.API.Domain;

public class Category
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }
}