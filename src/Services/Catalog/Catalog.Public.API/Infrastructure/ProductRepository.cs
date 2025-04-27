namespace Catalog.Public.API.Infrastructure;

public class ProductRepository(IDistributedCache distributedCache)
{
    public async Task<IEnumerable<ProductDto>> GetProductsAsync(Guid? categoryId = null)
    {
        var json = await distributedCache.GetStringAsync("Products");

        var list = (!string.IsNullOrEmpty(json)
            ? JsonSerializer.Deserialize<List<ProductDto>>(json)
            : []) ?? [];

        if (categoryId.HasValue)
            list = [.. list.Where(x => x.Categories.Any(c => c.Id == categoryId.Value))];

        return list;
    }

    public async Task AddAsync(Product product)
    {
        var list = (await GetProductsAsync()).ToList();

        list.Add(product.Adapt<ProductDto>());

        var json = JsonSerializer.Serialize(list);

        await distributedCache.SetStringAsync("Products", json);
    }
}
