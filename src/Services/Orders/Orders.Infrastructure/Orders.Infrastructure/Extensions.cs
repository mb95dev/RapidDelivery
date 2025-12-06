using Common.CQRS;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Abstractions;
using Orders.Infrastructure.Contexts;
using Orders.Infrastructure.Decorators;

namespace Orders.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructureServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services
            .AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));


        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        services.TryDecorate(typeof(ICommandHandler<,>), typeof(OutboxCommandHandlerDecorator<,>));

        services.TryDecorate(typeof(INotificationHandler<>), typeof(OutboxEventHandlerDecorator<>));

        return services;
    }



}
