using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Dtos.ShoppingCart
{
    public class OrderItemDto
    {
        public long TourId { get; set; }
        public string TourName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
