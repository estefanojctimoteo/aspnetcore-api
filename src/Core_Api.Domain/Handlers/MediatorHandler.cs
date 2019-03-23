using System.Threading.Tasks;
using Core_Api.Domain.Core.Commands;
using Core_Api.Domain.Core.Events;
using Core_Api.Domain.Interfaces;
using MediatR;

namespace Core_Api.Domain.Handlers
{
    public class MediatorHandler : IMediatorHandler
    {
        private readonly IMediator _mediator;
        private readonly IEventStore _eventStore;

        public MediatorHandler(IMediator mediator, IEventStore eventStore)
        {
            _mediator = mediator;
            _eventStore = eventStore;
        }

        public async Task SendCommand<T>(T command) where T : Command
        {
            await _mediator.Send(command);
        }

        public async Task PublishEvent<T>(T theEvent) where T : Event
        {
            if (!theEvent.MessageType.Equals("DomainNotification"))
                _eventStore?.SaveEvent(theEvent);

            await _mediator.Publish(theEvent);
        }
    }
}