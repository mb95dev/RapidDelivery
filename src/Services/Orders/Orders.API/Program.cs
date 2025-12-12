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
    .AddHostedService<OutboxProcessor>()
    .AddGrpc(options =>
    {
        options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    })
    .AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });

var app = builder.Build();

app.UseCors();
app.MapCarter();

// Map gRPC service
app.MapGrpcService<Orders.API.Services.OrderGrpcService>();

// Enable gRPC-Web for browser clients
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

if(app.Environment.IsDevelopment())
{
    await app.InitDbAsync();
}

app.Run();
