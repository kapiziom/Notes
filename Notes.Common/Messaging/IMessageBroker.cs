using Notes.Common.Messaging.Messages;

namespace Notes.Common.Messaging
{
    public interface IMessageBroker
    {
        Task<TResponse> SendCommandAsync<TResponse>(ICommand<TResponse> command, CancellationToken ct = default);

        Task<TResponse> SendQueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct = default);

        Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : IEvent;
    }
}