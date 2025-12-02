using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain.Shopping;
using Explorer.Tours.API.Public.Tourist;


namespace Explorer.Tours.Core.UseCases
{

    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _cartRepo;
        private readonly ITourRepository _tourRepo;

        public ShoppingCartService(IShoppingCartRepository cartRepo, ITourRepository tourRepo)
        {
            _cartRepo = cartRepo;
            _tourRepo = tourRepo;
        }

        public void AddToCart(long touristId, long tourId)
        {
            var tour = _tourRepo.GetPublishedById(tourId);
            if (tour == null)
                throw new ArgumentException("Tour does not exist or is not published.");

            var cart = _cartRepo.GetByTouristId(touristId);

            if (cart == null)
            {
                cart = new ShoppingCart(touristId);
                _cartRepo.Create(cart);
            }

            cart.AddItem(tour.Id, tour.Title, (decimal)tour.Price);
            _cartRepo.Update(cart);
        }
    }
}
