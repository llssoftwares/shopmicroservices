namespace Cart.API.Application.Commands;

public record AddItem(Guid ProductId, int Quantity)
{
    public Guid CartId { get; set; }
}

public record AddItemResponse(ShoppingCartDto Cart);

public class AddItemCommandHandler(ShoppingCartRepository shoppingCartRepository, ProductRepository productRepository)
{
    public async Task<AddItemResponse> HandleAsync(AddItem command)
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
