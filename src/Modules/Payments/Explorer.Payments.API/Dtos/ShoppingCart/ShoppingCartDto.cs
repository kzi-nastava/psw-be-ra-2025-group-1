namespace Explorer.Payments.API.Dtos.ShoppingCart
{
    public class ShoppingCartDto
    {
        public long TouristId { get; set; }

        public decimal Subtotal { get; set; }   
        public decimal CouponDiscount { get; set; } 
        public decimal TotalPrice { get; set; }   
        
        public string? CouponCode { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }
}