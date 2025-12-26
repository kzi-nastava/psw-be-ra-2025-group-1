using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Dtos.ShoppingCart;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain.Coupons;
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

        private readonly ICouponRepository _couponRepo;
        private readonly ICouponRedemptionRepository _couponRedemptionRepo;

        public ShoppingCartService(
            IShoppingCartRepository cartRepo,
            ITourRepository tourRepo,
            ITourPurchaseTokenRepository tokenRepo,
            ICouponRepository couponRepo,
            ICouponRedemptionRepository couponRedemptionRepo)
        {
            _cartRepo = cartRepo;
            _tourRepo = tourRepo;
            _tokenRepo = tokenRepo;

            _couponRepo = couponRepo;
            _couponRedemptionRepo = couponRedemptionRepo;
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

            // (opciono) ako želiš da popust uvek bude “fresh” kad se menja korpa:
            RecalculateCouponDiscountIfApplied(cart);

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
                    Subtotal = 0,
                    CouponCode = null,
                    CouponDiscount = 0,
                    TotalPrice = 0
                };
            }

            return new ShoppingCartDto
            {
                TouristId = cart.TouristId,

                Subtotal = cart.Subtotal,
                CouponCode = cart.AppliedCouponCode,
                CouponDiscount = cart.TotalDiscount,
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
            
            RecalculateCouponDiscountIfApplied(cart);

            _cartRepo.Update(cart);
        }
        
        public void ApplyCoupon(long touristId, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Coupon code is required.");

            var cart = _cartRepo.GetByTouristId(touristId);
            if (cart == null)
            {
                cart = new ShoppingCart(touristId);
                cart = _cartRepo.Create(cart);
            }

            var coupon = ValidateCouponOrThrow(code, touristId);

            var discount = coupon.CalculateDiscount(cart.Subtotal);
            cart.ApplyCoupon(coupon.Code, discount);

            _cartRepo.Update(cart);
        }
        
        public void RemoveCoupon(long touristId)
        {
            var cart = _cartRepo.GetByTouristId(touristId);
            if (cart == null) return;

            cart.RemoveCoupon();
            _cartRepo.Update(cart);
        }

        public List<TourPurchaseTokenDto> Checkout(long touristId)
        {
            var cart = _cartRepo.GetByTouristId(touristId);
            if (cart == null || !cart.Items.Any())
                throw new InvalidOperationException("Shopping cart is empty.");
            
            Coupon? coupon = null;
            if (!string.IsNullOrWhiteSpace(cart.AppliedCouponCode))
            {
                coupon = ValidateCouponOrThrow(cart.AppliedCouponCode!, touristId);
                
                var discount = coupon.CalculateDiscount(cart.Subtotal);
                cart.ApplyCoupon(coupon.Code, discount);
            }

            var createdTokens = new List<TourPurchaseToken>();

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
                    continue;

                var token = new TourPurchaseToken(
                    item.TourId,
                    touristId,
                    DateOnly.FromDateTime(DateTime.UtcNow)
                );

                _tokenRepo.Create(token);
                createdTokens.Add(token);
            }
            
            if (coupon != null)
            {
                _couponRedemptionRepo.Create(new CouponRedemption(
                    coupon.Id,
                    touristId,
                    null,
                    DateTime.UtcNow
                ));

            }

            cart.Clear();
            _cartRepo.Update(cart);

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


        private Coupon ValidateCouponOrThrow(string code, long userId)
        {
            var normalized = Coupon.NormalizeCode(code);

            var coupon = _couponRepo.GetByCode(normalized);
            if (coupon == null)
                throw new ArgumentException("Kupon ne postoji.");

            var nowUtc = DateTime.UtcNow;
            if (!coupon.IsValidAt(nowUtc))
                throw new ArgumentException("Kupon nije aktivan ili je istekao.");

            if (coupon.MaxTotalUses is not null)
            {
                var total = _couponRedemptionRepo.CountTotalUses(coupon.Id);
                if (total >= coupon.MaxTotalUses.Value)
                    throw new ArgumentException("Kupon je dostigao maksimalan broj korišćenja.");
            }

            if (coupon.MaxUsesPerUser is not null)
            {
                var perUser = _couponRedemptionRepo.CountUsesForUser(coupon.Id, userId);
                if (perUser >= coupon.MaxUsesPerUser.Value)
                    throw new ArgumentException("Kupon si već iskoristio maksimalan broj puta.");
            }

            return coupon;
        }


        private void RecalculateCouponDiscountIfApplied(ShoppingCart cart)
        {
            if (string.IsNullOrWhiteSpace(cart.AppliedCouponCode)) return;

            // ako kupon više nije validan, možeš ili da baciš grešku, ili da ga skineš.
            // ovde ga samo osvežimo ako postoji, a ako ne postoji/istekao -> skidamo
            var normalized = Coupon.NormalizeCode(cart.AppliedCouponCode!);
            var coupon = _couponRepo.GetByCode(normalized);

            if (coupon == null || !coupon.IsValidAt(DateTime.UtcNow))
            {
                cart.RemoveCoupon();
                return;
            }

            var discount = coupon.CalculateDiscount(cart.Subtotal);
            cart.ApplyCoupon(coupon.Code, discount);
        }
    }
}
