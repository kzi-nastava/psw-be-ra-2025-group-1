using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Dtos.ShoppingCart;

namespace Explorer.Payments.API.Public.Tourist
{
    public interface IShoppingCartService
    {
        void AddToCart(long touristId, long tourId);
        void RemoveFromCart(long touristId, long tourId);
        ShoppingCartDto GetCart(long touristId);
        List<TourPurchaseTokenDto> Checkout(long touristId);
        void ApplyCoupon(long touristId, string code);
        void RemoveCoupon(long touristId);

    }
}
