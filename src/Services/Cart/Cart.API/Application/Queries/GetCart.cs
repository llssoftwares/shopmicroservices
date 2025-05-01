namespace Cart.API.Application.Queries;

public record GetCart(Guid Id) : IQuery<GetCartResponse>;

public record GetCartResponse(ShoppingCartDto Cart);

public class GetCartHandler(ShoppingCartRepository shoppingCartRepository)
    : IQueryHandler<GetCart, GetCartResponse>
{
    public async ValueTask<GetCartResponse> Handle(GetCart query, CancellationToken cancellationToken)
    {
        var shoppingCart = await shoppingCartRepository.GetAsync(query.Id);

        return new(shoppingCart.Adapt<ShoppingCartDto>());
    }
}
