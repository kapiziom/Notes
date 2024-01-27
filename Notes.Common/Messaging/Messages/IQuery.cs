using MediatR;

namespace Notes.Common.Messaging.Messages
{
    public interface IQuery<out TRequestResult> : IRequest<TRequestResult>
    {
    }
}