using System;
using System.Collections.Generic;
using Core_Api.Domain.Core.Events;

namespace Core_Api.Infra.Data.Repository.EventSourcing
{
    public interface IEventStoreRepository : IDisposable
    {
        void Store(StoredEvent theEvent);
        IList<StoredEvent> All(Guid aggregateId);
    }
}