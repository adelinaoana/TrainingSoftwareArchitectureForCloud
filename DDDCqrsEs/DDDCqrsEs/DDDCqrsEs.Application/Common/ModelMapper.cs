using DDDCqrsEs.Application.Commands.StockCommands;
using DDDCqrsEs.Domain.Models;
using DDDCqrsEs.Domain.Projections;
using System;

namespace DDDCqrsEs.Application.Common
{
    public class ModelMapper
    {
        public static StockModel ConvertCommandToModel(UpdateStockCommand request)
        {
            return new StockModel()
            {
                LicensePlate = request.LicensePlate,
                Item = request.Item,
                Location = request.Location,
                QuantityOnHand = request.QuantityOnHand,
                Status = request.Status,
                Lot = request.Lot,
                BestBeforeDate = request.BestBeforeDate,
                CountryOfOrigin = request.CountryOfOrigin
            };
        }

        public static StockModel ConvertCommandToModel(CreateStockCommand request)
        {
            return new StockModel()
            {
                LicensePlate = request.LicensePlate,
                Item = request.Item,
                Location = request.Location,
                QuantityOnHand = request.QuantityOnHand,
                Status = request.Status,
                Lot = request.Lot,
                BestBeforeDate = request.BestBeforeDate,
                CountryOfOrigin = request.CountryOfOrigin
            };
        }
        public static StockProjection MapFromModel(Guid id, StockModel stockModel)
        {
            StockProjection stock = new StockProjection()
            {
                Id = id,
                LicensePlate = stockModel.LicensePlate,
                Item = stockModel.Item,
                Location = stockModel.Location,
                Lot = stockModel.Lot,
                Status = stockModel.Status,
                BestBeforeDate = stockModel.BestBeforeDate,
                CountryOfOrigin = stockModel.CountryOfOrigin,
                QuantityOnHand = stockModel.QuantityOnHand
            };
            return stock;
        }

        public static void MapModelIntoProjection(StockProjection projection, StockModel model)
        {
            projection.LicensePlate = model.LicensePlate;
            projection.Item = model.Item;
            projection.Lot = model.Lot;
            projection.QuantityOnHand = model.QuantityOnHand;
            projection.Status = model.Status;
            projection.Location = model.Location;
            projection.BestBeforeDate = model.BestBeforeDate;
            projection.CountryOfOrigin = model.CountryOfOrigin;
        }

    }
}
