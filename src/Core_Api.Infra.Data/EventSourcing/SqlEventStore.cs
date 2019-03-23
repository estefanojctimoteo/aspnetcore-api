using Core_Api.Domain.Core.Events;
using Core_Api.Domain.Interfaces;
using Core_Api.Infra.Data.Repository.EventSourcing;
using Newtonsoft.Json;

namespace Core_Api.Infra.Data.EventSourcing
{
    public class SqlEventStore : IEventStore
    {
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IUser _user;

        public SqlEventStore(IEventStoreRepository eventStoreRepository, IUser usuario)
        {
            _eventStoreRepository = eventStoreRepository;
            _user = usuario;
        }

        public void SaveEvent<T>(T evento) where T : Event
        {
            var serializedData = JsonConvert.SerializeObject(evento);

            var storedEvent = new StoredEvent(
                evento,
                serializedData,
                _user.GetUserId().ToString());

            _eventStoreRepository.Store(storedEvent);
        }
    }
}