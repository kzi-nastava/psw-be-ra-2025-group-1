using Explorer.Payments.Core.Domain.Cupons;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface ICouponRepository
    {
        Coupon? GetByCode(string codeNormalized);
        Coupon Create(Coupon coupon);
        void Update(Coupon coupon);
    }
}