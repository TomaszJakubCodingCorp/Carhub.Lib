using Carhub.Lib.MessageBrokers.Abstractions;

namespace Carhub.Lib.MessageBrokers.RabbitMq.Conventions.Abstractions;

internal interface IConventionProvider
{
    IConvention Get<TMessage>() where TMessage : IEvent;
}