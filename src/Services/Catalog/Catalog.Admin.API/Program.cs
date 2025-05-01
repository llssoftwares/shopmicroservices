var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddApplicationServices()
    .AddDatabase(builder.Configuration)
    .AddEventBus(builder.Configuration)
    .AddCarter()
    .AddCustomExceptionHandler();

var app = builder.Build();

app.MapCarter();
app.MapOpenApi();
app.UseHttpsRedirection()
    .UseCustomExceptionHandler();

app.Run();