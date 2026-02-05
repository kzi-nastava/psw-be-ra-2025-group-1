using Explorer.Payments.API.Dtos.Coupons;

namespace Explorer.Payments.API.Public.Author
{
    public interface ICouponService
    {
        CouponDto Create(long authorId, CreateCouponRequestDto req);
    }
}