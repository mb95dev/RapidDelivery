using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Messaging.MassTransit;

public static class Extensions
{
    public static IServiceCollection AddMessageBroker
        (this IServiceCollection services, IConfiguration configuration)
    {


        return services.AddMassTransit(configure =>
        {
            configure.SetKebabCaseEndpointNameFormatter();

            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(configuration["MessageBroker:Host"]!), h =>
                {
                    h.Username(configuration["MessageBroker:Username"]!);
                    h.Password(configuration["MessageBroker:Password"]!);
                });

                cfg.ConfigureEndpoints(context);
            });

        });
        //Udekorować to w ten sposób/dodać structor? or anything etc 
        //    .TryDecorate(typeof(ICommandHandler<>), typeof(OutboxCommandHandlerDecorator<>));
        //.TryDecorate(typeof(IEventHandler<>), typeof(OutboxEventHandlerDecorator<>)); ;
    }

}
