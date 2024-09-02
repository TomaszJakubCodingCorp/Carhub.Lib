namespace Carhub.Lib.Cqrs.Commands.Abstractions;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}