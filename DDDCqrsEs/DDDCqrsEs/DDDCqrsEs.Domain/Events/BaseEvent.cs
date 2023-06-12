using DDDCqrsEs.Domain.Models;
using System;

namespace DDDCqrsEs.Domain.Events
{
    public class BaseEvent : IBaseEvent
    {
        public DateTime TimeStamp { get; private set; }
        public string EventType { get; set; }
        public Guid AggregateId { get; set; }
        public StockModel Stock { get; set; }
        public int Version { get; set; }

        public BaseEvent()
        {
            TimeStamp = DateTime.Now;
        }

        public BaseEvent(string eventType)
        {
            TimeStamp = DateTime.Now;
            EventType = eventType;
        }
    }
}
