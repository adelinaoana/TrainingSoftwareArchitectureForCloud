using DDDCqrsEs.Domain.Models;
using System;

namespace DDDCqrsEs.Domain.Events
{
    public class StockCreated : BaseEvent
    {
        public StockCreated() { }
        public StockCreated(Guid id, string licensePlate, string item, string location, int quantityOnHand,
                string status, string lot, DateTime bestBeforeDate, string countryOfOrigin) : base(nameof(StockCreated))
            {
                AggregateId = id;
                Stock = new StockModel
                {
                    LicensePlate = licensePlate,
                    Item = item,
                    Location = location,
                    QuantityOnHand = quantityOnHand,
                    Status = status,
                    Lot = lot,
                    BestBeforeDate = bestBeforeDate,
                    CountryOfOrigin = countryOfOrigin
                };
            }

        public StockCreated(StockModel stock, Guid aggregateId) : base(nameof(StockCreated))
        {
            Stock = stock;
            AggregateId = aggregateId;
        }
    }

    public class StockUpdated : BaseEvent
    {
        public StockUpdated() { }
        public StockUpdated(Guid id, string licensePlate, string item, string location, int quantityOnHand,
            string status, string lot, DateTime bestBeforeDate, string countryOfOrigin) : base(nameof(StockUpdated))
        {
            AggregateId = id;
            Stock = new StockModel
            {
                LicensePlate = licensePlate,
                Item = item,
                Location = location,
                QuantityOnHand = quantityOnHand,
                Status = status,
                Lot = lot,
                BestBeforeDate = bestBeforeDate,
                CountryOfOrigin = countryOfOrigin
            };
        }
        public StockUpdated(StockModel stock, Guid aggregateId) : base(nameof(StockCreated))
        {
            Stock = stock;
            AggregateId = aggregateId;
        }
    }

    public class StockDeleted : BaseEvent
    {
        public StockDeleted() { }
        public StockDeleted(Guid id) : base(nameof(StockDeleted))
        {
            AggregateId = id;
        }
    }
}

