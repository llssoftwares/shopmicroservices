namespace Catalog.Public.API.Application.Products.Queries;

public record GetProducts(Guid? CategoryId) : IQuery<GetProductsResponse>;

public record GetProductsResponse(IEnumerable<ProductDto> Products);

public class GetProductsHandler(ProductRepository productRepository)
    : IQueryHandler<GetProducts, GetProductsResponse>
{
    public async ValueTask<GetProductsResponse> Handle(GetProducts query, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetProductsAsync(query.CategoryId);

        return new(products.Select(x => x.Adapt<ProductDto>()));
    }
}