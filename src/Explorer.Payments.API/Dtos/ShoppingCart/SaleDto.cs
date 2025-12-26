using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Dtos.ShoppingCart
{
    public class SaleDto
    {
        public long Id { get; set; }
        public DateTime PurchasedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public List<SaleItemDto> Items { get; set; } = new();
    }
}
