namespace Cart.API.Application.Dtos;

public record ShoppingCartItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal Price);