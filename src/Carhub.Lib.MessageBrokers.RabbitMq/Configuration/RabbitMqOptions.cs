namespace Carhub.Lib.MessageBrokers.RabbitMq.Configuration;

internal sealed record RabbitMqOptions
{
    internal static string OptionsName = $"{nameof(RabbitMqOptions)}";
    public int Port { get; init; }
    public string HostName { get; init; }
    public string VirtualHost { get; init; }
    public string UserName { get; init; }
    public string Password { get; init; }
    public string ConnectionName { get; init; }
    public List<ConsumerOptions> Consumers { get; init; }
}