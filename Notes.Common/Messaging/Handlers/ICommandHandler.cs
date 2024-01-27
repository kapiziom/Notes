using MediatR;
using Notes.Common.Messaging.Messages;

namespace Notes.Common.Messaging.Handlers
{
    public interface ICommandHandler<in TCommand, TRequestResult> : IRequestHandler<TCommand, TRequestResult>
        where TCommand : ICommand<TRequestResult>
    {
        new Task<TRequestResult> Handle(TCommand command, CancellationToken ct = default);
    }
}