using DDDCqrsEs.Application.Common;
using DDDCqrsEs.Domain.Projections;
using System;
using System.Collections.Generic;

namespace DDDCqrsEs.Application.Queries.StockQueris
{
    public class GetAllStocksQuery : BaseRequest<GetAllStocksQueryResponse>
    {
    }

    public class GetAllStocksQueryResponse
    {
        public IEnumerable<StockProjection> Stocks { get; set; }
    }

    public class GetStockByIdQuery : BaseRequest<GetStockByIdQueryResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetStockByIdQueryResponse
    {
        public StockProjection Stock { get; set; }
    }

}
