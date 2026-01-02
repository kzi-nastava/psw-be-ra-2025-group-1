namespace Explorer.Payments.Core.Domain.Cupons;

public class CouponRedemption
{
    public long Id { get; private set; }

    public long CouponId { get; private set; }
    public long UserId { get; private set; }

    public long? PurchaseId { get; private set; }     // vežeš kad napraviš TourPurchase
    public DateTime RedeemedAtUtc { get; private set; }

    private CouponRedemption() { }

    public CouponRedemption(long couponId, long userId, long? purchaseId, DateTime redeemedAtUtc)
    {
        CouponId = couponId;
        UserId = userId;
        PurchaseId = purchaseId;
        RedeemedAtUtc = redeemedAtUtc;
    }
}