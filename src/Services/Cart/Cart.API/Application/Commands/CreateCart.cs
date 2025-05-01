namespace Cart.API.Application.Commands;

public record CreateCart() : ICommand<CreateCartResponse>;

public record CreateCartResponse(ShoppingCartDto Cart);

public class CreateCartEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/cart", async (ISender sender) =>
        {
            var response = await sender.Send(new CreateCart());

            return Results.Created($"/cart/{response.Cart.Id}", response);
        })
        .WithTags("Cart")
        .WithSummary("Create cart");
    }
}

public class CreateCartHandler(ShoppingCartRepository shoppingCartRepository)
    : ICommandHandler<CreateCart, CreateCartResponse>
{
    public async ValueTask<CreateCartResponse> Handle(CreateCart command, CancellationToken cancellationToken)
    {
        var shoppingCart = await shoppingCartRepository.CreateAsync();

        return new(shoppingCart.Adapt<ShoppingCartDto>());
    }
}