using Carhub.Lib.Cqrs.Commands.Abstractions;
using Carhub.Lib.Cqrs.Queries.Abstractions;

namespace Carhub.Lib.Cqrs.Dispatchers.Abstractions;

public interface ICqrsDispatcher
{
    Task HandleAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;

    Task<TResult> HandleAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}