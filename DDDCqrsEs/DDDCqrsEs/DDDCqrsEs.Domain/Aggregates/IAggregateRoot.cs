using DDDCqrsEs.Domain.Events;
using System;
using System.Collections.Generic;

namespace DDDCqrsEs.Domain.Aggregates
{
    public interface IAggregateRoot
    {
        public Guid Id { get; set; }
        public void AddHandler<T>(Action<T> action) where T : IBaseEvent;
        public void AddEvent(IBaseEvent _event);
        public void ReconstituteFromEvents(List<BaseEvent> events);
    }
}
