using System;

namespace DDDCqrsEs.Domain.Projections
{
    public class StockProjection
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
}
