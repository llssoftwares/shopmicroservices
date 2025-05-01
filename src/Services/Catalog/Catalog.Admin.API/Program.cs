var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddApplicationServices()
    .AddDatabase(builder.Configuration)
    .AddEventBus(builder.Configuration)
    .AddCustomExceptionHandler();

var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection()
    .UseCustomExceptionHandler();

app.MapGet("/products", async (ISender sender, [FromQuery] Guid[] categoriesIds) =>
{
    var response = await sender.Send(new GetProducts() { CategoriesIds = categoriesIds });
    return Results.Ok(response);
})
.WithName("GetProducts");

app.MapPost("/products", async (ISender sender, CreateProduct command) =>
{
    var response = await sender.Send(command);

    return Results.Created($"/products/{response.ProductId}", response);
})
.WithName("CreateProduct");

app.MapGet("/categories", async (ISender sender) =>
{
    var response = await sender.Send(new GetCategories());
    return Results.Ok(response);
})
.WithName("GetCategories");

app.MapPost("/categories", async (ISender sender, CreateCategory command) =>
{
    var response = await sender.Send(command);
    return Results.Created($"/categories/{response.CategoryId}", response);
})
.WithName("CreateCategory");

app.Run();