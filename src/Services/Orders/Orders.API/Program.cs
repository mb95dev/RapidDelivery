using Carter;
using Orders.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddCarter()
    .AddInfrastructure(builder.Configuration);


var app = builder.Build();

app.MapCarter();