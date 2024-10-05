using RabbitMQ.Client;

namespace Carhub.Lib.MessageBrokers.RabbitMq.Connections;

internal sealed class ConsumerConnection(IConnection connection)
{
    public IConnection Connection { get; } = connection;
}