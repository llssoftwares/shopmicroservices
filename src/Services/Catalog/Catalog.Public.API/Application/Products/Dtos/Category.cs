namespace Catalog.Public.API.Application.Products.Dtos;

public class CategoryDto
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }
}