namespace Cart.API.Infrastructure;

public class ShoppingCartRepository(IDistributedCache distributedCache)
{
    private static string BuildCacheKey(Guid id) => $"ShoppingCart/{id}";

    public async Task<ShoppingCart> CreateAsync()
    {
        var shoppingCart = new ShoppingCartDto(Guid.NewGuid(), null, [], 0);

        await distributedCache.SetStringAsync(
            BuildCacheKey(shoppingCart.Id),
            JsonSerializer.Serialize(shoppingCart),
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(1)
            });

        return shoppingCart.Adapt<ShoppingCart>();
    }

    public async Task<ShoppingCart> GetAsync(Guid id)
    {
        var json = await distributedCache.GetStringAsync(BuildCacheKey(id));

        var dto = json != null
            ? JsonSerializer.Deserialize<ShoppingCartDto>(json)!
            : throw new CartNotFoundException(id);

        var shoppingCart = new ShoppingCart
        {
            Id = dto.Id,
            UserId = dto.UserId
        };

        foreach (var item in dto.Items)
        {
            shoppingCart.AddItem(new ShoppingCartItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Price = item.Price,
                Quantity = item.Quantity
            });
        }

        return shoppingCart;
    }

    public async Task SaveAsync(ShoppingCart shoppingCart)
    {
        await distributedCache.SetStringAsync(
            BuildCacheKey(shoppingCart.Id),
            JsonSerializer.Serialize(shoppingCart.Adapt<ShoppingCartDto>()),
            new DistributedCacheEntryOptions
            {
                SlidingExpiration = !shoppingCart.UserId.HasValue
                ? TimeSpan.FromDays(1)
                : null,
            });
    }
}