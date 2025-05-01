namespace Catalog.Admin.API.Application.Categories.Commands;

public record CreateCategory(string Name, string? Description) : ICommand<CreateCategoryResponse>;

public record CreateCategoryResponse(Guid CategoryId);

public class CreateCategoryValidator : AbstractValidator<CreateCategory>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");        
    }
}

public class CreateCategoryHandler(CatalogAdminDbContext dbContext)
    : ICommandHandler<CreateCategory, CreateCategoryResponse>
{
    public async ValueTask<CreateCategoryResponse> Handle(CreateCategory command, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description
        };

        await dbContext.Categories.AddAsync(category, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new(category.Id);
    }
}