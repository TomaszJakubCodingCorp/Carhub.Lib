using Carhub.Lib.MessageBrokers.RabbitMq.Connections;
using Carhub.Lib.MessageBrokers.RabbitMq.Conventions.Abstractions;
using Carhub.Lib.MessageBrokers.RabbitMq.Serializing;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Carhub.Lib.MessageBrokers.RabbitMq.Clients;

internal sealed class RabbitMqClient : IRabbitMqClient
{
    private readonly ILogger<RabbitMqClient> _logger;
    private readonly bool _loggerEnabled;
    private readonly IConnection _connection;
    private readonly IRabbitMqSerializer _serializer;

    public RabbitMqClient(
        ILogger<RabbitMqClient> logger,
        ProducerConnection producerConnection,
        IRabbitMqSerializer serializer)
    {
        _logger = logger;
        _connection = producerConnection.Connection;
        _serializer = serializer;
    }
    
    public void Publish<TMessage>(TMessage message, IConvention convention, Guid? messageId = null) where TMessage : class
    {
        var channel = _connection.CreateModel();
        var body = _serializer.ToJson(message);
        var properties = CreateMessageProperties(channel, messageId);
        channel.BasicPublish(convention.Exchange, convention.RoutingKey, properties, body.ToArray());
        if (_loggerEnabled)
        {
            _logger.LogInformation("Published message with ID: {id} to EXCHANGE: '{exchange}' with routing key: '{routingKey}'",
                properties.MessageId, convention.Exchange,convention.RoutingKey);
        }
    }

    private static IBasicProperties CreateMessageProperties(IModel channel, Guid? messageId)
    {
        var properties = channel.CreateBasicProperties();
        properties.MessageId = messageId is null ? Guid.NewGuid().ToString() : messageId.ToString();
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.Now.ToUnixTimeSeconds());
        return properties;
    }
}