using Carter;
using Orders.Application;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddCarter()
    .AddApplicationServices(builder.Configuration);
    


var app = builder.Build();

app.MapCarter();