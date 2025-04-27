var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddApplicationServices()
    .AddRedis(builder.Configuration)
    .AddEventBus(builder.Configuration)
    .AddEventHandlers()
    .AddCustomExceptionHandler();

var app = builder.Build();

app.MapOpenApi();
app.UseHttpsRedirection()
    .UseCustomExceptionHandler();

await app.UseEventHandlers();

app.MapGet("/cart/{id:guid}", async ([FromServices] GetCartQueryHandler handler, Guid id) =>
{
    var response = await handler.HandleAsync(new GetCart(id));

    return Results.Created($"/cart/{response.Cart.Id}", response);
})
.WithName("GetCart");

app.MapPost("/cart", async ([FromServices] CreateCartCommandHandler handler) =>
{
    var response = await handler.HandleAsync(new CreateCart());

    return Results.Created($"/cart/{response.Cart.Id}", response);
})
.WithName("CreateCart");

app.MapPost("/cart/{id}/items", async ([FromServices] AddItemCommandHandler handler, Guid id, AddItem command) =>
{
    command.CartId = id;

    var response = await handler.HandleAsync(command);

    return Results.Ok(response);
})
.WithName("SaveCart");

app.Run();