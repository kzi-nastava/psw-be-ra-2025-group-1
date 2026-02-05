using Explorer.Payments.Core.Domain.Cupons;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface ICouponRedemptionRepository
    {
        int CountTotalUses(long couponId);
        int CountUsesForUser(long couponId, long userId);

        CouponRedemption Create(CouponRedemption redemption);
    }
}