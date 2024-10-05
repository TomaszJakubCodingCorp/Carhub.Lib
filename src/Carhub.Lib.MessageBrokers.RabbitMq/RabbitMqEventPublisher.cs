using Carhub.Lib.MessageBrokers.Abstractions;
using Carhub.Lib.MessageBrokers.RabbitMq.Clients;
using Carhub.Lib.MessageBrokers.RabbitMq.Conventions.Abstractions;

namespace Carhub.Lib.MessageBrokers.RabbitMq;

internal sealed class RabbitMqEventPublisher(
    IConventionProvider conventionProvider,
    IRabbitMqClient client) : IEventPublisher
{
    public Task Publish<TMessage>(TMessage message) where TMessage : class, IEvent
    {
        var convention = conventionProvider.Get<TMessage>();
        client.Publish(message, convention, Guid.NewGuid());
        return Task.CompletedTask;
    }
}