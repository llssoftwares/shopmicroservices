namespace Cart.API.Application.Dtos;

public record ShoppingCartDto(
    Guid Id,
    Guid? UserId,
    List<ShoppingCartItem> Items,
    decimal TotalPrice);