using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain.Shopping;

public class ShoppingCart : AggregateRoot
{
    private readonly List<OrderItem> _items = new();

    public long TouristId { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    // ✅ Kupon
    public string? AppliedCouponCode { get; private set; }
    public decimal CouponDiscount { get; private set; } // snapshot (izračunat u trenutku apply)

    // ✅ Cene
    public decimal Subtotal => _items.Sum(i => i.TotalPrice);

    // da ne ode u minus ako je kupon > subtotal
    public decimal TotalDiscount => Math.Min(CouponDiscount, Subtotal);

    // ukupno za naplatu (posle kupona)
    public decimal TotalPrice => Subtotal - TotalDiscount;

    protected ShoppingCart() { }

    public ShoppingCart(long touristId)
    {
        TouristId = touristId;
    }

    public void AddItem(long tourId, string name, decimal price)
    {
        var existing = _items.FirstOrDefault(i => i.TourId == tourId);
        if (existing != null)
        {
            existing.IncreaseQuantity();
            return;
        }

        _items.Add(new OrderItem(tourId, name, price, 1));
    }

    public void RemoveItem(long tourId)
    {
        var item = _items.SingleOrDefault(i => i.TourId == tourId);
        if (item == null) return;

        _items.Remove(item);
        // kupon ostaje, ali TotalDiscount je clampovan na Subtotal (ne može u minus)
    }

    // ✅ Apply/Remove kupona
    public void ApplyCoupon(string couponCodeNormalized, decimal discountAmount)
    {
        if (string.IsNullOrWhiteSpace(couponCodeNormalized))
            throw new ArgumentException("Coupon code is required.", nameof(couponCodeNormalized));

        AppliedCouponCode = couponCodeNormalized.Trim().ToUpperInvariant();
        CouponDiscount = discountAmount < 0 ? 0 : discountAmount;
    }

    public void RemoveCoupon()
    {
        AppliedCouponCode = null;
        CouponDiscount = 0;
    }

    public void Clear()
    {
        _items.Clear();
        RemoveCoupon(); // kad se korpa očisti, nema smisla da kupon ostane
    }
}
