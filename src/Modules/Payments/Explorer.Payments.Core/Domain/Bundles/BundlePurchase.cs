using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain.Bundles;

public class BundlePurchase : Entity
{
    public long TouristId { get; private set; }
    public long BundleId { get; private set; }
    public decimal PricePaid { get; private set; }
    public DateTime PurchaseDate { get; private set; }

    protected BundlePurchase() { }

    public BundlePurchase(long touristId, long bundleId, decimal pricePaid)
    {
        if (touristId <= 0) throw new ArgumentException("Invalid tourist ID");
        if (bundleId <= 0) throw new ArgumentException("Invalid bundle ID");
        if (pricePaid < 0) throw new ArgumentException("Price cannot be negative");

        TouristId = touristId;
        BundleId = bundleId;
        PricePaid = pricePaid;
        PurchaseDate = DateTime.UtcNow;
    }
}
