namespace Cart.API.Domain;

public class ShoppingCartItem
{
    public required Guid ProductId { get; init; }

    public required string ProductName { get; init; }

    public int Quantity { get; set; }

    public decimal Price { get; init; }
}