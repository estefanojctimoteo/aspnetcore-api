using System;
using Core_Api.Domain.Core.Events;

namespace Core_Api.Domain.AppUsers.Events
{
    public class AppUserRemovedEvent : Event
    {
        public Guid Id { get; protected set; }

        public AppUserRemovedEvent(Guid id)
        {
            Id = id;
        }
    }
}