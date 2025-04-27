namespace Cart.API.Application.Events;

public class ProductCreatedHandler(ProductRepository productRepository) : IEventHandler<ProductCreated>
{
    public async Task HandleAsync(ProductCreated evt)
    {
        var product = new Product
        {
            Id = evt.Id,
            Name = evt.Name,
            Price = evt.Price
        };

        await productRepository.AddAsync(product);
    }
}
