using System;
using Core_Api.Domain.Core.Commands;

namespace Core_Api.Domain.AppUsers.Commands
{
    public class RegisterAppUserCommand : Command
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }

        public RegisterAppUserCommand(Guid id, string email)
        {
            Id = id;
            Email = email;
        }
    }
}