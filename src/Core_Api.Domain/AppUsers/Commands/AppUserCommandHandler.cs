using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core_Api.Domain.Core.Notifications;
using Core_Api.Domain.Handlers;
using Core_Api.Domain.Interfaces;
using Core_Api.Domain.AppUsers.Events;
using Core_Api.Domain.AppUsers.Repository;
using MediatR;

namespace Core_Api.Domain.AppUsers.Commands
{
    public class AppUserCommandHandler : CommandHandler,
             IRequestHandler<RegisterAppUserCommand>,
             IRequestHandler<RemoveAppUserCommand>
    {
        private readonly IMediatorHandler _mediator;
        private readonly IAppUserRepository _appUserRepository;

        public AppUserCommandHandler(
            IUnitOfWork uow,
            INotificationHandler<DomainNotification> notifications,
            IAppUserRepository appUserRepository, IMediatorHandler mediator) : base(uow, mediator, notifications)
        {
            _appUserRepository = appUserRepository;
            _mediator = mediator;
        }

        public Task Handle(RegisterAppUserCommand message, CancellationToken cancellationToken)
        {
            var appUser = new AppUser(message.Id, message.Email);

            if (!appUser.IsValid())
            {
                NotificarValidacoesErro(appUser.ValidationResult);
                return Task.CompletedTask;
            }

            var appUserCore = _appUserRepository.GetById(appUser.Id);

            if (appUserCore != null && appUserCore.Id == appUser.Id)
            {
                _mediator.PublishEvent(new DomainNotification(message.MessageType, "An user with this email already exists."));
                return Task.CompletedTask;
            }

            _appUserRepository.Add(appUser);
            _mediator.PublishEvent(new AppUserRegisteredEvent(appUser.Id, appUser.Email));

            return Task.CompletedTask;
        }
        public Task Handle(RemoveAppUserCommand message, CancellationToken cancellationToken)
        {
            // Business rules here
            _appUserRepository.Remove(message.Id);
            _mediator.PublishEvent(new AppUserRemovedEvent(message.Id));

            return Task.CompletedTask;
        }
    }
}