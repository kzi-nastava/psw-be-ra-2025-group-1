using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.Services;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Dtos.ShoppingCart;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.Coupons;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Domain.Shopping;
using Explorer.Payments.Core.Domain.TourPurchaseTokens;


namespace Explorer.Payments.Core.UseCases
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _cartRepo;
        private readonly ITourBrowsingInfo _tourBrowsingInfo;
        private readonly ITourPurchaseTokenRepository _tokenRepo;
        private readonly ITourPurchaseRepository _purchaseRepo;
        private readonly ICouponRepository _couponRepo;
        private readonly ICouponRedemptionRepository _couponRedemptionRepo;
        private readonly ISaleService _saleService;
        private readonly IWalletRepository _walletRepo;
        private readonly IBundleRepository _bundleRepo;
        private readonly IBundlePurchaseRepository _bundlePurchaseRepo;

        public ShoppingCartService(
            IShoppingCartRepository cartRepo,
            ITourBrowsingInfo tourBrowsingInfo,
            ITourPurchaseTokenRepository tokenRepo,
            ITourPurchaseRepository purchaseRepo,
            ICouponRepository couponRepo,
            ICouponRedemptionRepository couponRedemptionRepo,
            ISaleService saleService,
            IWalletRepository walletRepo,
            IBundleRepository bundleRepo,
            IBundlePurchaseRepository bundlePurchaseRepo)
        {
            _cartRepo = cartRepo;
            _tourBrowsingInfo = tourBrowsingInfo;
            _tokenRepo = tokenRepo;
            _purchaseRepo = purchaseRepo;
            _couponRepo = couponRepo;
            _couponRedemptionRepo = couponRedemptionRepo;
            _saleService = saleService;
            _walletRepo = walletRepo;
            _bundleRepo = bundleRepo;
            _bundlePurchaseRepo = bundlePurchaseRepo;
        }

        public void AddToCart(long touristId, long tourId)
        {
            var tour = _tourBrowsingInfo.GetPublishedTourById(tourId);
            if (tour == null || !tour.IsPublished)
                throw new ArgumentException("Tour does not exist or is not published.");

            var cart = GetOrCreateCart(touristId);

            // ✅ Apply sale discount if available
            var priceToUse = (decimal)tour.Price;
            var activeSales = _saleService.GetActiveSalesForTour(tourId);
            if (activeSales.Any())
            {
                var bestSale = activeSales.OrderByDescending(s => s.DiscountPercentage).First();
                priceToUse = (decimal)(tour.Price * (1 - bestSale.DiscountPercentage / 100.0));
            }

            cart.AddTour(tour.Id, tour.Title, priceToUse);
            RecalculateCouponDiscountIfApplied(cart);

            _cartRepo.Update(cart);
        }

        public void AddBundleToCart(long touristId, long bundleId)
        {
            var bundle = _bundleRepo.Get(bundleId);
            if (bundle == null || bundle.Status != Explorer.Payments.Core.Domain.Bundles.BundleStatus.Published)
                throw new ArgumentException("Bundle does not exist or is not published.");

            var cart = GetOrCreateCart(touristId);

            cart.AddBundle(bundle.Id, bundle.Name, bundle.Price);
            RecalculateCouponDiscountIfApplied(cart);

            _cartRepo.Update(cart);
        }

        private ShoppingCart GetOrCreateCart(long touristId)
        {
            var cart = _cartRepo.GetByTouristId(touristId);
            if (cart == null)
            {
                cart = new ShoppingCart(touristId);
                cart = _cartRepo.Create(cart);
            }
            return cart;
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
                    BundleId = i.BundleId,
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

            cart.RemoveTour(tourId);

            RecalculateCouponDiscountIfApplied(cart);

            _cartRepo.Update(cart);
        }

        public void RemoveBundleFromCart(long touristId, long bundleId)
        {
            var cart = _cartRepo.GetByTouristId(touristId);
            if (cart == null) return;

            cart.RemoveBundle(bundleId);

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
            Wallet userWallet;
            try
            {
                userWallet = _walletRepo.GetByTouristId(touristId);
            }
            catch (NotFoundException)
            {
                // Create wallet if it doesn't exist
                userWallet = new Wallet(touristId, 0);
                userWallet = _walletRepo.Create(userWallet);
            }

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
                if (item.TourId.HasValue)
                {
                    var tour = _tourBrowsingInfo.GetPublishedTourById(item.TourId.Value);
                    if (tour == null)
                        throw new InvalidOperationException("Tour does not exist or is not published.");

                    if (_tokenRepo.ExistsForUserAndTour(touristId, item.TourId.Value))
                        continue;

                    // Calculate final price with Sale + Coupon
                    var finalPrice = CalculateFinalPrice(item.TourId.Value, item.Price, cart.Subtotal, cart.TotalDiscount, cart.Items.Count);

                    if (finalPrice > (decimal)userWallet.Balance)
                        continue;

                    // Update wallet
                    userWallet.Update(userWallet.Balance - (double)finalPrice);
                    _walletRepo.Update(userWallet);

                    // Create TourPurchase record
                    var purchase = new TourPurchase(touristId, item.TourId.Value, finalPrice);
                    _purchaseRepo.Create(purchase);

                    // Create token
                    var token = new TourPurchaseToken(
                        item.TourId.Value,
                        touristId,
                        DateOnly.FromDateTime(DateTime.UtcNow)
                    );

                    _tokenRepo.Create(token);
                    createdTokens.Add(token);
                }
                else if (item.BundleId.HasValue)
                {
                    var bundle = _bundleRepo.Get(item.BundleId.Value);
                    if (bundle == null) throw new InvalidOperationException("Bundle not found.");

                    if (item.Price > (decimal)userWallet.Balance)
                        continue;

                    // Update wallet
                    userWallet.Update(userWallet.Balance - (double)item.Price);
                    _walletRepo.Update(userWallet);

                    // Create BundlePurchase
                    var bundlePurchase = new Explorer.Payments.Core.Domain.Bundles.BundlePurchase(touristId, bundle.Id, item.Price);
                    _bundlePurchaseRepo.Create(bundlePurchase);

                    // Create tokens for all tours
                    foreach (var tourId in bundle.TourIds)
                    {
                        if (_tokenRepo.ExistsForUserAndTour(touristId, tourId))
                            continue;

                        var token = new TourPurchaseToken(tourId, touristId, DateOnly.FromDateTime(DateTime.UtcNow));
                        _tokenRepo.Create(token);
                        createdTokens.Add(token);
                    }
                }
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

        private decimal CalculateFinalPrice(long tourId, decimal basePrice, decimal cartSubtotal, decimal cartTotalDiscount, int itemCount)
        {
            // 1. Apply Sale discount first
            var activeSales = _saleService.GetActiveSalesForTour(tourId);
            var salePrice = basePrice;

            if (activeSales.Any())
            {
                var bestSale = activeSales.OrderByDescending(s => s.DiscountPercentage).First();
                salePrice = basePrice * (1 - bestSale.DiscountPercentage / 100m);
            }

            // 2. Apply proportional Coupon discount (if any)
            if (cartTotalDiscount > 0 && cartSubtotal > 0)
            {
                var proportionalCouponDiscount = (basePrice / cartSubtotal) * cartTotalDiscount;
                salePrice -= proportionalCouponDiscount;
            }

            return Math.Max(0, salePrice); // Ne može biti negativna cena
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
