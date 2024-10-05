using Carhub.Lib.MessageBrokers.RabbitMq.Configuration;
using Carhub.Lib.MessageBrokers.RabbitMq.Connections;
using Carhub.Lib.MessageBrokers.RabbitMq.Serializing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Carhub.Lib.MessageBrokers.RabbitMq.Consumers;

public static class ConsumerRegistryExtension
{
    
    public static IServiceCollection AddConsumer<TMessage>(this IServiceCollection services,
        Func<TMessage, Task> func)
        where TMessage : class
    {
        services
            .AddHostedService<RabbitMqBackgroundService<TMessage>>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RabbitMqBackgroundService<TMessage>>>();
                var consumerConnection = sp.GetRequiredService<ConsumerConnection>();
                var rabbitMqSerializer = sp.GetRequiredService<IRabbitMqSerializer>();
                var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                var typeName = typeof(TMessage).Name;
                var consumerOptions = options.Consumers?.FirstOrDefault(x
                    => x.Type == typeName);

                if (consumerOptions is null)
                {
                    throw new ArgumentException($"Consumer options for type: {typeName}");
                }

                return new RabbitMqBackgroundService<TMessage>(
                    logger,
                    consumerConnection,
                    rabbitMqSerializer,
                    consumerOptions,
                    func);
            });
        return services;
    }
}