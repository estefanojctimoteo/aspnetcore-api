using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Core_Api.Domain.AppUsers.Events
{
    public class AppUserEventHandler : 
        INotificationHandler<AppUserRegisteredEvent>,
        INotificationHandler<AppUserRemovedEvent>
    {
        public Task Handle(AppUserRegisteredEvent message, CancellationToken cancellationToken)
        {
            // TODO: Send an email?
            return Task.CompletedTask;
        }
        public Task Handle(AppUserRemovedEvent message, CancellationToken cancellationToken)
        {
            // TODO: Send an email?
            return Task.CompletedTask;
        }
    }
}