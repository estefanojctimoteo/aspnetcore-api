using System;
using Core_Api.Domain.Core.Commands;

namespace Core_Api.Domain.AppUsers.Commands
{
    public class RemoveAppUserCommand : Command
    {
        public Guid Id { get; protected set; }

        public RemoveAppUserCommand(Guid id)
        {
            Id = id;
        }
    }
}