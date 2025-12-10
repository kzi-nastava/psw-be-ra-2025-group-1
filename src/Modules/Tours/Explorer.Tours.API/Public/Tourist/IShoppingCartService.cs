using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface IShoppingCartService
    {
        void AddToCart(long touristId, long tourId);
        void RemoveFromCart(long touristId, long tourId);
        ShoppingCartDto GetCart(long touristId);
        void Checkout(long touristId);
    }
}
