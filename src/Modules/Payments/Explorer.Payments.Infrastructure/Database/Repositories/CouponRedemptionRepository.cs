using Explorer.Payments.Core.Domain.Cupons;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class CouponRedemptionRepository : ICouponRedemptionRepository
    {
        private readonly PaymentsContext _context;

        public CouponRedemptionRepository(PaymentsContext context)
        {
            _context = context;
        }

        public int CountTotalUses(long couponId)
        {
            return _context.CouponRedemptions.Count(x => x.CouponId == couponId);
        }

        public int CountUsesForUser(long couponId, long userId)
        {
            return _context.CouponRedemptions.Count(x => x.CouponId == couponId && x.UserId == userId);
        }

        public CouponRedemption Create(CouponRedemption redemption)
        {
            _context.CouponRedemptions.Add(redemption);
            _context.SaveChanges();
            return redemption;
        }
    }
}