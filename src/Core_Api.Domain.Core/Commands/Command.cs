using System;
using Core_Api.Domain.Core.Events;
using MediatR;

namespace Core_Api.Domain.Core.Commands
{
    public class Command : Message, IRequest
    {
        public DateTime Timestamp { get; private set; }

        public Command()
        {
            Timestamp = DateTime.Now;
        }
    }
}