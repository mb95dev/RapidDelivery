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
         services
             .AddDbContext<OrdersDbContext>(options =>
                 options.UseInMemoryDatabase("OrdersDatabase")) 
             .BuildServiceProvider();

         services.AddScoped<IApplicationDbContext, OrdersDbContext>();

         return services;
    }



}
