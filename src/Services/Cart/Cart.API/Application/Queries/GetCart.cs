namespace Cart.API.Application.Queries;

public record GetCart(Guid Id) : IQuery<GetCartResponse>;

public record GetCartResponse(ShoppingCartDto Cart);

public class GetCartEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/cart/{id:guid}", async (ISender sender, Guid id) =>
        {
            var response = await sender.Send(new GetCart(id));

            return Results.Created($"/cart/{response.Cart.Id}", response);
        })
        .WithTags("Cart")
        .WithSummary("Get cart");
    }
}

public class GetCartHandler(ShoppingCartRepository shoppingCartRepository)
    : IQueryHandler<GetCart, GetCartResponse>
{
    public async ValueTask<GetCartResponse> Handle(GetCart query, CancellationToken cancellationToken)
    {
        var shoppingCart = await shoppingCartRepository.GetAsync(query.Id);

        return new(shoppingCart.Adapt<ShoppingCartDto>());
    }
}
