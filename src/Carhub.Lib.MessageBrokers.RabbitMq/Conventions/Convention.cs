using Carhub.Lib.MessageBrokers.RabbitMq.Conventions.Abstractions;

namespace Carhub.Lib.MessageBrokers.RabbitMq.Conventions;

internal sealed class Convention(string exchange, string routingKey) : IConvention
{
    public string Exchange { get; } = exchange;
    public string RoutingKey { get; } = routingKey;
}