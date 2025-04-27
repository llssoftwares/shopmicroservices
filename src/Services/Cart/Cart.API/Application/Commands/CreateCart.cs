namespace Cart.API.Application.Commands;

public record CreateCart();

public record CreateCartResponse(ShoppingCartDto Cart);

public class CreateCartCommandHandler(ShoppingCartRepository shoppingCartRepository)
{
    public async Task<CreateCartResponse> HandleAsync(CreateCart _)
    {
        var shoppingCart = await shoppingCartRepository.CreateAsync();

        return new(shoppingCart.Adapt<ShoppingCartDto>());
    }
}
