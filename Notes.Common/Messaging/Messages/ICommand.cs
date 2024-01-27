using MediatR;

namespace Notes.Common.Messaging.Messages
{
    public interface ICommand<out TRequestResult> : IRequest<TRequestResult>
    {
    }
}