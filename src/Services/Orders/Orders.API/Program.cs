using Carter;
using Orders.Application;
using Orders.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddCarter()
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapCarter();
app.Run();
