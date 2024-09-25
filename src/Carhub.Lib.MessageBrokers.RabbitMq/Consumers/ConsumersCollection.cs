using Microsoft.Extensions.DependencyInjection;

namespace Carhub.Lib.MessageBrokers.RabbitMq.Consumers;

public sealed class ConsumersCollection
{
    private readonly Dictionary<Type, object> _consumers = [];

    public void AddConsumer<TMessage>(Func<TMessage, Task> handler)
    {
        if (_consumers.TryAdd(typeof(TMessage), handler))
        {
            throw new ArgumentException("Can not add consumer");
        }
    }

    internal void Build(IServiceCollection services)
    {
        foreach (var consumer in _consumers)
        {
            var methodInfo = typeof(ConsumerRegistryExtension).GetMethod(nameof(ConsumerRegistryExtension.AddConsumer));
            if (methodInfo is null)
            {
                throw new ArgumentException("Method not found");
            }
            var genericMethod = methodInfo.MakeGenericMethod(consumer.Key);
            genericMethod.Invoke(null, new[] { services, consumer.Value });
        }
    }
}