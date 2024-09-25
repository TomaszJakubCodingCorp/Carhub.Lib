using System.Text;
using Carhub.Lib.MessageBrokers.RabbitMq.Configuration;
using Carhub.Lib.MessageBrokers.RabbitMq.Connections;
using Carhub.Lib.MessageBrokers.RabbitMq.Serializing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Carhub.Lib.MessageBrokers.RabbitMq.Consumers;

internal sealed class RabbitMqBackgroundService<TMessage> : BackgroundService where TMessage : class
{
    private readonly ILogger<RabbitMqBackgroundService<TMessage>> _logger;
    private readonly IModel _channel;
    private readonly IRabbitMqSerializer _rabbitMqSerializer;
    private readonly ConsumerOptions _consumerOptions;
    private readonly Func<TMessage, Task> _handle;
    
    public RabbitMqBackgroundService(
        ILogger<RabbitMqBackgroundService<TMessage>> logger, 
        ConsumerConnection consumerConnection,
        IRabbitMqSerializer rabbitMqSerializer,
        ConsumerOptions consumerOptions,
        Func<TMessage,Task> handle)
    {
        _logger = logger;
        var connection = consumerConnection.Connection;
        _channel = connection.CreateModel();
        _rabbitMqSerializer = rabbitMqSerializer;
        _consumerOptions = consumerOptions;
        _handle = handle;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel.ExchangeDeclare(_consumerOptions.Exchange, ExchangeType.Direct, true, false);
        _channel.QueueDeclare(_consumerOptions.Queue, true, false, false);
        _channel.QueueBind(_consumerOptions.Queue, _consumerOptions.Exchange, _consumerOptions.Routing);
        
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            try
            {
                var messageBytes = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(messageBytes);
                var message = _rabbitMqSerializer.ToObject<TMessage>(json);
                await _handle(message);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };
        _channel.BasicConsume(queue: _consumerOptions.Queue,
            autoAck: false,
            consumer: consumer);
        return Task.CompletedTask;
    }
}