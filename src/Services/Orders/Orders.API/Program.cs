using Carter;
using Orders.API.BackgroundServices;
using Orders.Application;
using Orders.Infrastructure;
using Orders.Infrastructure.DbExtensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddCarter()
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices(builder.Configuration)
    .AddHostedService<OutboxProcessor>();

var app = builder.Build();

app.MapCarter();

if(app.Environment.IsDevelopment())
{
    await app.InitDbAsync();
}

app.Run();
