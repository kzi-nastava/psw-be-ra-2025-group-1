using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain.Shopping;
using Explorer.Tours.Core.Domain.Shopping;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{

    public class ShoppingCartDbRepository : IShoppingCartRepository
    {
        private readonly ToursContext _db;

        public ShoppingCartDbRepository(ToursContext db)
        {
            _db = db;
        }

        public ShoppingCart? GetByTouristId(long touristId)
        {
            return _db.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.TouristId == touristId);
        }

        public ShoppingCart Create(ShoppingCart cart)
        {
            _db.ShoppingCarts.Add(cart);
            _db.SaveChanges();
            return cart;
        }

        public ShoppingCart Update(ShoppingCart cart)
        {
            _db.ShoppingCarts.Update(cart);
            _db.SaveChanges();
            return cart;
        }
    }
}
