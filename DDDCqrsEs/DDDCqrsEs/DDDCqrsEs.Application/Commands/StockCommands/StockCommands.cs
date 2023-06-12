using DDDCqrsEs.Application.Common;
using System;

namespace DDDCqrsEs.Application.Commands.StockCommands
{
    public class CreateStockCommand : BaseRequest<CreateStockCommandResponse>
    {
        public string LicensePlate { get; set; }
        public string Item { get; set; }
        public string Location { get; set; }
        public int QuantityOnHand { get; set; }
        public string Status { get; set; }
        public string Lot { get; set; }
        public DateTime BestBeforeDate { get; set; }
        public string CountryOfOrigin { get; set; }
    }

    public class CreateStockCommandResponse
    {
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
    }


    public class UpdateStockCommand : BaseRequest<UpdateStockCommandResponse>
    {
        public Guid Id { get; set; }
        public string LicensePlate { get; set; }
        public string Item { get; set; }
        public string Location { get; set; }
        public int QuantityOnHand { get; set; }
        public string Status { get; set; }
        public string Lot { get; set; }
        public DateTime BestBeforeDate { get; set; }
        public string CountryOfOrigin { get; set; }
        public int Version { get; set; }
    }

    public class UpdateStockCommandResponse
    {
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
    }

    public class DeleteStockCommand : BaseRequest<DeleteStockCommandResponse>
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
    }
    public class DeleteStockCommandResponse
    {
        public Guid AggregateId { get; set; }
        public int Version { get; set; }

    }
}
