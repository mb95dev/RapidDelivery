using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Abstractions;
using Orders.Infrastructure.Contexts;

namespace Orders.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructureServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        
        services
            .AddDbContext<OrdersDbContext>(options =>
                options.UseSqlServer(connectionString));
         

         services.AddScoped<IApplicationDbContext, OrdersDbContext>();

         return services;
    }



}
