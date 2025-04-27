namespace Catalog.Public.API.Application.Products.Events;

public class ProductCreatedHandler(ProductRepository productRepository) : IEventHandler<ProductCreated>
{
    public async Task HandleAsync(ProductCreated evt)
    {
        var product = new Product
        {
            Id = evt.Id,
            Name = evt.Name,
            Price = evt.Price,
            Categories = [.. evt.CategoriesNames.Select(x => new Category
            {
                Id = x.Key,
                Name = x.Value
            })]
        };

        await productRepository.AddAsync(product);
    }
}
