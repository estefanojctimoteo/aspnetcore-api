using System;
using Core_Api.Domain.Core.Events;

namespace Core_Api.Domain.AppUsers.Events
{
    public class AppUserRegisteredEvent : Event
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }

        public AppUserRegisteredEvent(Guid id, string email)
        {
            Id = id;
            Email = email;
        }
    }
}