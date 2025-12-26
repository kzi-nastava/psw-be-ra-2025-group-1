using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Payments.Core.Domain.Shopping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Payments.Core.Domain
{
    public class SaleHistory : AggregateRoot
    {
        public long TouristId { get; private set; }
        public DateTime PurchasedAt { get; private set; }
        public decimal TotalPrice { get; private set; }

        // virtual property, EF Core koristi direktno
        public virtual List<SaleHistoryItem> Items { get; set; } = new();

        protected SaleHistory() { }

        public static SaleHistory CreateFromCart(ShoppingCart cart)
        {
            var sale = new SaleHistory
            {
                TouristId = cart.TouristId,
                PurchasedAt = DateTime.UtcNow,
                TotalPrice = cart.TotalPrice
            };

            foreach (var item in cart.Items)
            {
                sale.Items.Add(new SaleHistoryItem(item.TourId, item.TourName, item.Price, item.Quantity));
            }

            return sale;
        }
    }
}
