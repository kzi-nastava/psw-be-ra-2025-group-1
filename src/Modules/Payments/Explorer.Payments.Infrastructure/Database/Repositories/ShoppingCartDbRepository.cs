using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Domain.Shopping;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{

    public class ShoppingCartDbRepository : IShoppingCartRepository
    {
        private readonly PaymentsContext _db;

        public ShoppingCartDbRepository(PaymentsContext db)
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
