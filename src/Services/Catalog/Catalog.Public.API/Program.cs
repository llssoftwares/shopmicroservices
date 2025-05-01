var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddApplicationServices()
    .AddRedis(builder.Configuration)
    .AddEventBus(builder.Configuration)
    .AddEventHandlers()
    .AddCarter()
    .AddCustomExceptionHandler();

var app = builder.Build();

app.MapCarter();
app.MapOpenApi();
app.UseHttpsRedirection()
    .UseCustomExceptionHandler();

await app.UseEventHandlers();

app.Run();