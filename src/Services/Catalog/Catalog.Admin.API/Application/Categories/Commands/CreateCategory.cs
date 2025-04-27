namespace Catalog.Admin.API.Application.Categories.Commands;

public record CreateCategory(string Name, string? Description);

public record CreateCategoryResponse(Guid CategoryId);

public class CreateCategoryCommandHandler(CatalogAdminDbContext dbContext)
{
    public async Task<CreateCategoryResponse> HandleAsync(CreateCategory command)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description
        };

        await dbContext.Categories.AddAsync(category);

        await dbContext.SaveChangesAsync();

        return new(category.Id);
    }
}