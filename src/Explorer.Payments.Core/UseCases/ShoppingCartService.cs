using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Dtos.ShoppingCart;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Domain.Shopping;
using Explorer.Payments.Core.Domain.TourPurchaseTokens;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _cartRepo;
        private readonly ITourRepository _tourRepo;
        private readonly ITourPurchaseTokenRepository _tokenRepo;
        private readonly ISaleRepository _saleRepository;

        public ShoppingCartService(
            IShoppingCartRepository cartRepo,
            ITourRepository tourRepo,
            ITourPurchaseTokenRepository tokenRepo,
            ISaleRepository saleRepository)
        {
            _cartRepo = cartRepo;
            _tourRepo = tourRepo;
            _tokenRepo = tokenRepo;
            _saleRepository = saleRepository;
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
                cart = _cartRepo.Create(cart);
            }

            cart.AddItem(tour.Id, tour.Title, (decimal)tour.Price);
            _cartRepo.Update(cart);
        }

        public ShoppingCartDto GetCart(long touristId)
        {
            var cart = _cartRepo.GetByTouristId(touristId);
            if (cart == null) 
            {
                return new ShoppingCartDto
                {
                    TouristId = touristId,
                    Items = new List<OrderItemDto>(),
                    TotalPrice = 0
                };
            }

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

        public List<TourPurchaseTokenDto> Checkout(long touristId)
        {
            var cart = _cartRepo.GetByTouristId(touristId);
            if (cart == null || !cart.Items.Any())
                throw new InvalidOperationException("Shopping cart is empty.");

            var createdTokens = new List<TourPurchaseToken>();

            // Kreiranje Sale (istorija prodaje)
            var sale = Sale.CreateFromCart(cart);
            _saleRepository.Add(sale); // ← ovo ti treba da registruješ u DI

            foreach (var item in cart.Items)
            {
                var tour = _tourRepo.Get(item.TourId);
                if (tour == null)
                    throw new InvalidOperationException("Tour does not exist.");

                if (tour.Status == TourStatus.Archived)
                    throw new InvalidOperationException("Archived tour cannot be purchased.");

                if (tour.Status != TourStatus.Published)
                    throw new InvalidOperationException("Only published tours can be purchased.");

                if (_tokenRepo.ExistsForUserAndTour(touristId, item.TourId))
                    continue; // već kupljena

                var token = new TourPurchaseToken(
                    item.TourId,
                    touristId,
                    DateOnly.FromDateTime(DateTime.UtcNow)
                );

                _tokenRepo.Create(token);
                createdTokens.Add(token);
            }

            // Prazni korpu
            cart.Clear();
            _cartRepo.Update(cart);

            // Mapiranje tokena u DTO
            return createdTokens.Select(t => new TourPurchaseTokenDto
            {
                Id = t.Id,
                TourId = t.TourId,
                UserId = t.UserId,
                PurchaseDate = t.PurchaseDate,
                Status = t.Status.ToString(),
                IsValid = t.IsValid
            }).ToList();
        }

    }
}