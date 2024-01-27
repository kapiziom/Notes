using MediatR;
using Notes.Common.Messaging;
using Notes.Common.Messaging.Messages;

namespace Notes.WebAPI.Infrastructure.Messaging
{
    public class MediatR : IMessageBroker
    {
        public MediatR(IMediator mediator)
        {
            Mediator = mediator;
        }

        private IMediator Mediator { get; }

        public async Task<TResponse> SendCommandAsync<TResponse>(ICommand<TResponse> command, 
            CancellationToken ct = default) =>
            await Mediator.Send(command, ct);

        public async Task<TResponse> SendQueryAsync<TResponse>(IQuery<TResponse> query,
            CancellationToken ct = default) =>
            await Mediator.Send(query, ct);

        public async Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken ct = default)
            where TEvent : IEvent =>
            await Mediator.Publish(@event, ct);
    }
}