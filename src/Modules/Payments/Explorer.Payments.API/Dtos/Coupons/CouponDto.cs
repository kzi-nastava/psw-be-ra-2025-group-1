namespace Explorer.Payments.API.Dtos.Coupons
{
    public class CouponDto
    {
        public long Id { get; set; }
        public long AuthorId { get; set; }
        public long? TourId { get; set; }

        public string Code { get; set; } = string.Empty;
        public decimal Percentage { get; set; }

        public DateTime ValidFromUtc { get; set; }
        public DateTime? ValidToUtc { get; set; }

        public bool IsActive { get; set; }

        public int? MaxTotalUses { get; set; }
        public int? MaxUsesPerUser { get; set; }
    }
}