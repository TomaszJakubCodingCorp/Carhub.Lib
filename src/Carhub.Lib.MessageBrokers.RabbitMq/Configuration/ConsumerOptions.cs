namespace Carhub.Lib.MessageBrokers.RabbitMq.Configuration;

internal sealed record ConsumerOptions
{
    public string Type { get; init; }
    public string Exchange { get; init; }
    public string Routing { get; init; }
    public string Queue { get; init; }
}