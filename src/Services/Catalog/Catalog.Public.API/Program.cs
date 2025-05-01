var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
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
    .UseCustomExceptionHandler()
    .UseSwagger()
    .UseSwaggerUI();

await app.UseEventHandlers();

app.Run();