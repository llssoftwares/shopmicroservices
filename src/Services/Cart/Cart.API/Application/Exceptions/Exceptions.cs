namespace Cart.API.Application.Exceptions;

public class CartNotFoundException(Guid id) : NotFoundException("Cart", id);

public class ProductNotFoundException(Guid id) : NotFoundException("Product", id);