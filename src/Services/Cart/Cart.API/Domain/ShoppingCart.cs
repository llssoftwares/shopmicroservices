namespace Cart.API.Domain;

public class ShoppingCart
{
    private readonly List<ShoppingCartItem> _items = [];

    public required Guid Id { get; init; }

    public Guid? UserId { get; init; }

    public IReadOnlyCollection<ShoppingCartItem> Items => _items;

    public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);

    public void AddItem(ShoppingCartItem shoppingCartItem)
    {
        var item = _items.SingleOrDefault(x => x.ProductId == shoppingCartItem.ProductId);

        if (item == null)
            _items.Add(shoppingCartItem);
        else
            item.Quantity += shoppingCartItem.Quantity;
    }
}