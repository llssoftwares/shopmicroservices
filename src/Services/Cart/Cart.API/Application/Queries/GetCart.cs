namespace Cart.API.Application.Queries;

public record GetCart(Guid Id);

public record GetCartResponse(ShoppingCartDto Cart);

public class GetCartQueryHandler(ShoppingCartRepository shoppingCartRepository)
{
    public async Task<GetCartResponse> HandleAsync(GetCart query)
    {
        var shoppingCart = await shoppingCartRepository.GetAsync(query.Id);

        return new(shoppingCart.Adapt<ShoppingCartDto>());
    }
}
