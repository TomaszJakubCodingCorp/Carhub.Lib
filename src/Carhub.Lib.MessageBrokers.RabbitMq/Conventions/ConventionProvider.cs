using Carhub.Lib.MessageBrokers.Abstractions;
using Carhub.Lib.MessageBrokers.RabbitMq.Configuration;
using Carhub.Lib.MessageBrokers.RabbitMq.Conventions.Abstractions;
using Microsoft.Extensions.Options;

namespace Carhub.Lib.MessageBrokers.RabbitMq.Conventions;

internal sealed class ConventionProvider(IOptions<RabbitMqOptions> options) : IConventionProvider
{
    private readonly string _connectionName = options.Value.ConnectionName;

    public IConvention Get<TMessage>() where TMessage : IEvent
    {
        var exchange = _connectionName;
        var routingKey = typeof(TMessage).Name;
        return new Convention(exchange, routingKey);
    }
}