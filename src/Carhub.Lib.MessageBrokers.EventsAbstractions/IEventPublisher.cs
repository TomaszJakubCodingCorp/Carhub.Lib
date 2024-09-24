namespace Carhub.Lib.MessageBrokers.Abstractions;

public interface IEventPublisher
{
    Task Publish<TMessage>(TMessage message) where TMessage : class, IEvent;
}