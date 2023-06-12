using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace DDDCqrsEs.Persistance.DataModel
{
    public class EventEntity : TableEntity
    {

        public EventEntity() { }
        public EventEntity(Guid stockId, int version) : base(stockId.ToString(), version.ToString())
        {

        }

        //public int Id { get; set; }
        public DateTime TimeCreated { get; set; }
        public string EventType { get; set; }
        //public Guid StockId { get; set; }
        public string Data { get; set; }
    }
}
