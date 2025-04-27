namespace Catalog.Admin.API.Application.Products.Dtos;

public record ProductDto(Guid Id, string Name, decimal Price, IEnumerable<Guid> CategoriesIds);