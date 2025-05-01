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

app.MapGet("/products", async (ISender sender, Guid? categoryId) =>
{
    var response = await sender.Send(new GetProducts(categoryId));

    return Results.Ok(response);
})
.WithName("GetProducts");

app.Run();