using DDDCqrsEs.Domain.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDDCqrsEs.Persistance.DataModel;

namespace DDDCqrsEs.Domain.Repositories
{
    public interface IEventStore
    {
        public IAsyncEnumerable<EventEntity> GetEventsByAggregateId(Guid id);
        public IAsyncEnumerable<EventEntity> GetAllEvents();
        public Task SaveEvent(BaseEvent _event);
    }
}
