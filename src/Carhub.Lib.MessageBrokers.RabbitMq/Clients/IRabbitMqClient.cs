using Carhub.Lib.MessageBrokers.RabbitMq.Conventions.Abstractions;

namespace Carhub.Lib.MessageBrokers.RabbitMq.Clients;

internal interface IRabbitMqClient
{
    void Publish<TMessage>(TMessage message, IConvention convention, Guid? messageId = null) where TMessage : class;
}