namespace Explorer.Payments.API.Dtos.Coupons
{
    public class CreateCouponRequestDto
    {
        public decimal Percentage { get; set; }          // (0-100]
        public DateTime? ValidToUtc { get; set; }        // opcionalno
        public long? TourId { get; set; }                // null => sve ture autora

        public int? MaxTotalUses { get; set; }
        public int? MaxUsesPerUser { get; set; }
    }
}