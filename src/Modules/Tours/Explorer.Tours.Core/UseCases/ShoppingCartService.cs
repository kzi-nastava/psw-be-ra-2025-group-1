using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain.Shopping;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.API.Dtos;


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


        public ShoppingCartDto GetCart(long touristId)
        {
            var cart = _cartRepo.GetByTouristId(touristId);
            if (cart == null) return new ShoppingCartDto
            {
                TouristId = touristId,
                Items = new List<OrderItemDto>(),
                TotalPrice = 0
            };

            return new ShoppingCartDto
            {
                TouristId = cart.TouristId,
                TotalPrice = cart.TotalPrice,
                Items = cart.Items.Select(i => new OrderItemDto
                {
                    TourId = i.TourId,
                    TourName = i.TourName,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        public void RemoveFromCart(long touristId, long tourId)
        {
            var cart = _cartRepo.GetByTouristId(touristId);
            if (cart == null) return;

            cart.RemoveItem(tourId);
            _cartRepo.Update(cart);
        }
    }
}
