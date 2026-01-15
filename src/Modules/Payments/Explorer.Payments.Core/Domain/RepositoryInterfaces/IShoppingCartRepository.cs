using Explorer.Tours.Core.Domain.Shopping;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IShoppingCartRepository
    {
        ShoppingCart? GetByTouristId(long touristId);
        ShoppingCart Create(ShoppingCart cart);
        ShoppingCart Update(ShoppingCart cart);
    }
}
