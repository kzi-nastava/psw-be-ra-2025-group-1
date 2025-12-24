using Explorer.Payments.Core.Domain.Shopping;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IShoppingCartRepository
    {
        ShoppingCart? GetByTouristId(long touristId);
        ShoppingCart Create(ShoppingCart cart);
        ShoppingCart Update(ShoppingCart cart);
    }
}
