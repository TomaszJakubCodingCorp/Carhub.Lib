using RabbitMQ.Client;

namespace Carhub.Lib.MessageBrokers.RabbitMq.Connections;

internal sealed class ProducerConnection(IConnection connection)
{
    public IConnection Connection { get; } = connection;
}