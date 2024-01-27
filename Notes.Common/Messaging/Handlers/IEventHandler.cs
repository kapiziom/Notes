using MediatR;
using Notes.Common.Messaging.Messages;

namespace Notes.Common.Messaging.Handlers
{
    public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IEvent
    {
        new Task Handle(TEvent @event, CancellationToken ct = default);
    }
}