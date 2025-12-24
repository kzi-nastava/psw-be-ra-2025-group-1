using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Payments.Core.Domain.Shopping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Payments.Core.Domain
{
    public class Sale : AggregateRoot
    {
        public long TouristId { get; private set; }
        public DateTime PurchasedAt { get; private set; }
        public decimal TotalPrice { get; private set; }

        // virtual property, EF Core koristi direktno
        public virtual List<SaleItem> Items { get; set; } = new();

        protected Sale() { }

        public static Sale CreateFromCart(ShoppingCart cart)
        {
            var sale = new Sale
            {
                TouristId = cart.TouristId,
                PurchasedAt = DateTime.UtcNow,
                TotalPrice = cart.TotalPrice
            };

            foreach (var item in cart.Items)
            {
                sale.Items.Add(new SaleItem(item.TourId, item.TourName, item.Price, item.Quantity));
            }

            return sale;
        }
    }
}
