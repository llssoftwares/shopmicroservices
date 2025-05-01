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

app.MapGet("/cart/{id:guid}", async (ISender sender, Guid id) =>
{
    var response = await sender.Send(new GetCart(id));

    return Results.Created($"/cart/{response.Cart.Id}", response);
})
.WithName("GetCart");

app.MapPost("/cart", async (ISender sender) =>
{
    var response = await sender.Send(new CreateCart());

    return Results.Created($"/cart/{response.Cart.Id}", response);
})
.WithName("CreateCart");

app.MapPost("/cart/{id}/items", async (ISender sender, Guid id, AddItem command) =>
{
    command.CartId = id;

    var response = await sender.Send(command);

    return Results.Ok(response);
})
.WithName("SaveCart");

app.Run();