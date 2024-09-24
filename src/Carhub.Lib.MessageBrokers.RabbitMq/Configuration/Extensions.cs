using Carhub.Lib.MessageBrokers.Abstractions;
using Carhub.Lib.MessageBrokers.RabbitMq.Consumers;
using Carhub.Lib.MessageBrokers.RabbitMq.Conventions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Carhub.Lib.MessageBrokers.RabbitMq.Configuration;

public static class Extensions
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration,
        Action<ConsumersCollection> consumers)
    {
        services
            .AddConsumersConfiguration(consumers)
            .AddConventions()
            .AddServices()
            .AddConfiguration(configuration);
        return services;
    }

    private static IServiceCollection AddConsumersConfiguration(this IServiceCollection services,
        Action<ConsumersCollection> consumers)
    {
        ConsumersCollection consumersCollection = new ConsumersCollection();
        consumers(consumersCollection);
        consumersCollection.Build(services);
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
        => services
            .AddScoped<IEventPublisher, RabbitMqEventPublisher>();

    private static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.OptionsName));
}