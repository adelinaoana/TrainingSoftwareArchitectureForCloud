using DDDCqrsEs.Common.Constants;
using DDDCqrsEs.Domain.Events;
using DDDCqrsEs.Domain.Models;
using System;
using System.Collections.Generic;

namespace DDDCqrsEs.Domain.Aggregates
{
    public class Stock : AggregateRoot
    {
        public string LicensePlate { get; set; }
        public string Item { get; set; }
        public string Location { get; set; }
        public int QuantityOnHand { get; set; }
        public string Status { get; set; }
        public string Lot { get; set; }
        public DateTime BestBeforeDate { get; set; }
        public string CountryOfOrigin { get; set; }
        
        public Stock(Guid id)
        {
            Events = new List<IBaseEvent>();
            Id = id;
            AddHandler<StockCreated>(Handle);
            AddHandler<StockUpdated>(Handle);
            AddHandler<StockDeleted>(Handle);

        }
        public StockCreated Create(StockModel model)
        {
            StockCreated stockCreatedEvent = new StockCreated
            (
                Id = Id,
                LicensePlate = model.LicensePlate,
                Item = model.Item,
                Location = model.Location,
                QuantityOnHand = model.QuantityOnHand,
                Status = model.Status,
                Lot = model.Lot,
                BestBeforeDate = model.BestBeforeDate,
                CountryOfOrigin = model.CountryOfOrigin
            );
            Handle(stockCreatedEvent);
            AddEvent(stockCreatedEvent);
            return stockCreatedEvent;
        }

        public StockUpdated Update(StockModel model)
        {
            StockUpdated stockUpdatedEvent = new StockUpdated
            (
                Id = Id,
                LicensePlate = model.LicensePlate,
                Item = model.Item,
                Location = model.Location,
                QuantityOnHand = model.QuantityOnHand,
                Status = model.Status,
                Lot = model.Lot,
                BestBeforeDate = model.BestBeforeDate,
                CountryOfOrigin = model.CountryOfOrigin
            );
            Handle(stockUpdatedEvent);
            AddEvent(stockUpdatedEvent);
            return stockUpdatedEvent;
        }

        public StockDeleted Delete()
        {
            StockDeleted stockDeletedEvent = new StockDeleted(
                Id = Id
                );
            Handle(stockDeletedEvent);
            AddEvent(stockDeletedEvent);
            return stockDeletedEvent;
        }
        private void Handle(StockUpdated _event)
        {
            LicensePlate = _event.Stock.LicensePlate;
            Item = _event.Stock.Item;
            Location = _event.Stock.Location;
            QuantityOnHand = _event.Stock.QuantityOnHand;
            Status = _event.Stock.Status;
            Lot = _event.Stock.Lot;
            BestBeforeDate = _event.Stock.BestBeforeDate;
            CountryOfOrigin = _event.Stock.CountryOfOrigin;
        }

        private void Handle(StockCreated _event)
        {
            LicensePlate = _event.Stock.LicensePlate;
            Item = _event.Stock.Item;
            Location = _event.Stock.Location;
            QuantityOnHand = _event.Stock.QuantityOnHand;
            Status = _event.Stock.Status;
            Lot = _event.Stock.Lot;
            BestBeforeDate = _event.Stock.BestBeforeDate;
            CountryOfOrigin = _event.Stock.CountryOfOrigin;
        }
        private void Handle(StockDeleted _event)
        {
            Status = StockStatusValues.CLOSED;
        }
    } 
}
