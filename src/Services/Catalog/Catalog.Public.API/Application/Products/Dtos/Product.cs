namespace Catalog.Public.API.Application.Products.Dtos;

public class ProductDto
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public decimal Price { get; init; }

    public ICollection<CategoryDto> Categories { get; init; } = [];
}