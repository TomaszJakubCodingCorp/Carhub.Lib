namespace Carhub.Lib.MessageBrokers.RabbitMq.Conventions.Abstractions;

internal interface IConvention
{
    public string Exchange { get; }
    public string RoutingKey { get; }
}