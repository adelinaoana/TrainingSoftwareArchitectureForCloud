using Newtonsoft.Json;
using DDDCqrsEs.Domain.Events;
using DDDCqrsEs.Domain.Models;
using DDDCqrsEs.Persistance.DataModel;
using System;
using System.Collections.Generic;

namespace DDDCqrsEs.Application.Common
{
    public class EventMapper
    {
        public static BaseEvent ConvertFromEntityToBase(EventEntity _event)
        {
            var stockData = JsonConvert.DeserializeObject<StockModel>(_event.Data);

            switch (_event.EventType)
            {
                case "StockCreated":
                    return new StockCreated(stockData, new Guid(_event.PartitionKey));
                case "StockUpdated":
                    return new StockUpdated(stockData, new Guid(_event.PartitionKey));
                case "StockDeleted":
                    return new StockDeleted(new Guid(_event.PartitionKey));
            }
            return new BaseEvent();
        }

        public static List<BaseEvent> ConvertListOfEntityEventsToBase(IEnumerable<EventEntity> _events)
        {
            List<BaseEvent> baseEvents = new List<BaseEvent>();
            foreach(var _event in _events)
            {
                baseEvents.Add(ConvertFromEntityToBase(_event));
            }
            return baseEvents;
        }

    }
}
