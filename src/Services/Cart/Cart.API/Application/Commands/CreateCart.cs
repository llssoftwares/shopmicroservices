namespace Cart.API.Application.Commands;

public record CreateCart() : ICommand<CreateCartResponse>;

public record CreateCartResponse(ShoppingCartDto Cart);

public class CreateCartHandler(ShoppingCartRepository shoppingCartRepository)
    : ICommandHandler<CreateCart, CreateCartResponse>
{
    public async ValueTask<CreateCartResponse> Handle(CreateCart command, CancellationToken cancellationToken)
    {
        var shoppingCart = await shoppingCartRepository.CreateAsync();

        return new(shoppingCart.Adapt<ShoppingCartDto>());
    }
}