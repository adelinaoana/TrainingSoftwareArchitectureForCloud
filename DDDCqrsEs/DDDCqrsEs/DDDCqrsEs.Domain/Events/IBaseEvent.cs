using MediatR;
using DDDCqrsEs.Domain.Models;
using System;

namespace DDDCqrsEs.Domain.Events
{
    public interface IBaseEvent : IRequest<Unit>
    {
        public DateTime TimeStamp { get; }
        public string EventType { get; set; }
        public Guid AggregateId { get; set; }
        public StockModel Stock { get; set; }
        public int Version { get; set; }
    }
}
