namespace Cart.API.Domain;

public class Product
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public decimal Price { get; init; }
}
