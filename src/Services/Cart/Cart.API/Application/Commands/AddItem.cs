namespace Cart.API.Application.Commands;

public record AddItem(Guid ProductId, int Quantity) : ICommand<AddItemResponse>
{
    public Guid CartId { get; set; }
}

public record AddItemResponse(ShoppingCartDto Cart);

public class AddItemValidator : AbstractValidator<AddItem>
{
    public AddItemValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");        
    }
}

public class AddItemEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/cart/{id}/items", async (ISender sender, Guid id, AddItem command) =>
        {
            command.CartId = id;

            var response = await sender.Send(command);

            return Results.Ok(response);
        })
        .WithTags("Cart")
        .WithSummary("Add item to cart");
    }
}

public class AddItemHandler(ShoppingCartRepository shoppingCartRepository, ProductRepository productRepository) 
    : ICommandHandler<AddItem, AddItemResponse>
{
    public async ValueTask<AddItemResponse> Handle(AddItem command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetAsync(command.ProductId);
        var shoppingCart = await shoppingCartRepository.GetAsync(command.CartId);

        shoppingCart.AddItem(new ShoppingCartItem
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Quantity = command.Quantity,
            Price = product.Price
        });

        await shoppingCartRepository.SaveAsync(shoppingCart);

        return new(shoppingCart.Adapt<ShoppingCartDto>());
    }
}
