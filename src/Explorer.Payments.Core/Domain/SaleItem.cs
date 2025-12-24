using Explorer.BuildingBlocks.Core.Domain;
using System.Collections.Generic;

namespace Explorer.Payments.Core.Domain
{
    public class SaleItem : ValueObject
    {
        public long Id { get; private set; }
        public long TourId { get; private set; }
        public string TourName { get; private set; }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }

        protected SaleItem() { }

        public SaleItem(long tourId, string tourName, decimal price, int quantity)
        {
            TourId = tourId;
            TourName = tourName;
            Price = price;
            Quantity = quantity;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TourId;
            yield return Price;
            yield return Quantity;
        }
    }
}
