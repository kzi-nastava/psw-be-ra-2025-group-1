using Explorer.Payments.Core.Domain.Cupons;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly PaymentsContext _context;

        public CouponRepository(PaymentsContext context)
        {
            _context = context;
        }

        public Coupon? GetByCode(string codeNormalized)
        {
            return _context.Coupons.FirstOrDefault(c => c.Code == codeNormalized);
        }

        public Coupon Create(Coupon coupon)
        {
            _context.Coupons.Add(coupon);
            _context.SaveChanges();
            return coupon;
        }

        public void Update(Coupon coupon)
        {
            _context.Coupons.Update(coupon);
            _context.SaveChanges();
        }
    }
}