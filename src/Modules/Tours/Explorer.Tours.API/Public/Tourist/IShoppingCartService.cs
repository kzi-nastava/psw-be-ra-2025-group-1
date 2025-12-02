using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface IShoppingCartService
    {
        void AddToCart(long touristId, long tourId);
    }
}
