namespace Catalog.Public.API.Application.Products.Queries;

public record GetProducts(Guid? CategoryId);

public record GetProductsResponse(IEnumerable<ProductDto> Products);

public class GetProductsHandler(ProductRepository productRepository)
{
    public async Task<GetProductsResponse> HandleAsync(GetProducts query)
    {
        var products = await productRepository.GetProductsAsync(query.CategoryId);

        return new(products.Select(x => x.Adapt<ProductDto>()));
    }
}
