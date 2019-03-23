using Core_Api.Domain.Core.Events;

namespace Core_Api.Domain.Interfaces
{
    public interface IEventStore
    {
        void SaveEvent<T>(T theEvent) where T : Event;
    }
}