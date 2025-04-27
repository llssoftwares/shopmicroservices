var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddApplicationServices()
    .AddDatabase(builder.Configuration)
    .AddEventBus(builder.Configuration);

var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection()
    .UseCustomExceptionHandler();

app.MapGet("/products", async ([FromServices] GetProductsQueryHandler handler, [FromQuery] Guid[] categoriesIds) =>
{
    var response = await handler.HandleAsync(new GetProducts() { CategoriesIds = categoriesIds });
    return Results.Ok(response);
})
.WithName("GetProducts");

app.MapPost("/products", async ([FromServices] CreateProductCommandHandler handler, CreateProduct command) =>
{
    var response = await handler.HandleAsync(command);

    return Results.Created($"/products/{response.ProductId}", response);
})
.WithName("CreateProduct");

app.MapGet("/categories", async ([FromServices] GetCategoriesQueryHandler handler) =>
{
    var response = await handler.HandleAsync(new GetCategories());
    return Results.Ok(response);
})
.WithName("GetCategories");

app.MapPost("/categories", async ([FromServices] CreateCategoryCommandHandler handler, CreateCategory command) =>
{
    var response = await handler.HandleAsync(command);
    return Results.Created($"/categories/{response.CategoryId}", response);
})
.WithName("CreateCategory");

app.Run();