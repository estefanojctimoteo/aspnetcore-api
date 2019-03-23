using System.Threading.Tasks;
using Core_Api.Domain.Core.Commands;
using Core_Api.Domain.Core.Events;

namespace Core_Api.Domain.Interfaces
{
    public interface IMediatorHandler
    {
        Task PublishEvent<T>(T theEvent) where T : Event;
        Task SendCommand<T>(T command) where T : Command;
    }
}