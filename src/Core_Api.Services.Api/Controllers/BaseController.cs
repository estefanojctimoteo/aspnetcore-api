using System;
using System.Linq;
using Core_Api.Domain.Core.Notifications;
using Core_Api.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace Core_Api.Services.Api.Controllers
{
    [Produces("application/json")]
    public abstract class BaseController : Controller
    {
        private readonly DomainNotificationHandler _notifications;
        private readonly IMediatorHandler _mediator;

        protected Guid AspNetUserId { get; set; }

        protected BaseController(IMediatorHandler mediator)
        {
            _mediator = mediator;
        }

        protected BaseController(INotificationHandler<DomainNotification> notifications, 
                                 IUser aspNetUser,
                                 IMediatorHandler mediator)
        {
            _notifications = (DomainNotificationHandler)notifications;
            _mediator = mediator;

            if (aspNetUser.IsAuthenticated())
                AspNetUserId = aspNetUser.GetUserId();
        }

        protected IActionResult Response_BadRequest(string msgErro)
        {
            return BadRequest(new
            {
                success = false,
                errors = msgErro
            });
        }

        protected IActionResult Response_BadRequest(string msgErro, object result = null)
        {
            return BadRequest(new
            {
                success = false,
                errors = msgErro,
                data = result
            });
        }

        protected new IActionResult Response(object result = null)
        {
            if (InvalidOperation())
            {
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = _notifications.GetNotifications().Select(n=>n.Value)
            });
        }

        protected bool InvalidOperation()
        {
            return (!_notifications.HasNotifications());
        }

        protected void NotifyErrorInvalidModel()
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                var errorMsg = error.Exception == null ? error.ErrorMessage : error.Exception.Message;
                NotifyError(string.Empty, errorMsg);
            }
        }

        protected void NotifyError(string codigo, string theMessage)
        {
            _mediator.PublishEvent(new DomainNotification(codigo, theMessage));
        }

        protected void AddIdentityErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                NotifyError(result.ToString(), error.Description);
            }
        }
    }
}