using Common.Messaging.MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Orders.Application;

    public static class Extensions
    {
        public static IServiceCollection AddApplicationServices
            (this IServiceCollection services, IConfiguration configuration)
        {

            return services.AddMessageBroker(configuration);
        }
    }

