var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddApplicationServices()
    .AddDatabase(builder.Configuration)
    .AddEventBus(builder.Configuration)
    .AddCarter()
    .AddCustomExceptionHandler();

var app = builder.Build();

app.MapCarter();
app.MapOpenApi();
app.UseHttpsRedirection()
    .UseCustomExceptionHandler()
    .UseSwagger()
    .UseSwaggerUI();

app.Run();