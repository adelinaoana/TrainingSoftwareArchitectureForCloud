using DDDCqrsEs.Application.Common;
using DDDCqrsEs.Common;
using DDDCqrsEs.Common.Constants;
using DDDCqrsEs.Domain.Models;
using DDDCqrsEs.Domain.Projections;
using DDDCqrsEs.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DDDCqrsEs.Persistance.Repositories
{
    [MapServiceDependency(Name: nameof(StockProjectionRepository))]
    public class StockProjectionRepository : IStockProjectionRepository
    {
        private ToDoDbContext dbContext;
        public StockProjectionRepository(ToDoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<StockProjection> GetAllStocks()
        {

            return dbContext.Stocks.ToList();
        }

        public StockProjection GetStockById(Guid id)
        {
            var stock = dbContext.Stocks.FirstOrDefault(s => s.Id == id);
            stock.BestBeforeDate = stock.BestBeforeDate.ToLocalTime();
            return stock;
        }

        public void CreateStock(StockProjection stock)
        {
            dbContext.Add(stock);
            dbContext.SaveChanges();
        }

        public void DeleteStock(Guid stockId)
        {

            var stockToBeDeleted = dbContext.Stocks.FirstOrDefault(s => s.Id == stockId);
            if (stockToBeDeleted != null)
            {
                stockToBeDeleted.Status = StockStatusValues.CLOSED;
                dbContext.SaveChanges();
            }

        }

        public void UpdateStock(Guid stockId, StockModel stock, int version)
        {

            var stockToBeUpdated = dbContext.Stocks.FirstOrDefault(s => s.Id == stockId);
            if (stockToBeUpdated != null)
            {
                ModelMapper.MapModelIntoProjection(stockToBeUpdated, stock);
                stockToBeUpdated.Version = version;
                dbContext.SaveChanges();
            }

        }

        public StockProjection GetStockByLicensePlate(string licensePlate)
        {

            var stock = dbContext.Stocks.FirstOrDefault(s => s.LicensePlate == licensePlate);
            return stock;

        }
    }
}
