namespace Cart.API.Infrastructure;

public class ProductRepository(IDistributedCache distributedCache)
{
    private static string BuildCacheKey(Guid id) => $"Product/{id}";

    public async Task<Product> GetAsync(Guid id)
    {
        var json = await distributedCache.GetStringAsync(BuildCacheKey(id));

        return json != null
            ? JsonSerializer.Deserialize<Product>(json)!
            : throw new ProductNotFoundException(id);
    }

    public async Task AddAsync(Product product)
    {
        await distributedCache.SetStringAsync(BuildCacheKey(product.Id), JsonSerializer.Serialize(product));
    }
}
