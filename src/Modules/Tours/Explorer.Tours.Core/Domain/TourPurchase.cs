using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class TourPurchase : Entity
{
    public long TouristId { get; private set; }
    public long TourId { get; private set; }
    public decimal PricePaid { get; private set; }
    public DateTime PurchaseDate { get; private set; }

    // For EF Core
    protected TourPurchase() { }

    public TourPurchase(long touristId, long tourId, decimal pricePaid)
    {
        if (touristId <= 0) throw new ArgumentException("Invalid tourist ID");
        if (tourId <= 0) throw new ArgumentException("Invalid tour ID");
        if (pricePaid < 0) throw new ArgumentException("Price cannot be negative");

        TouristId = touristId;
        TourId = tourId;
        PricePaid = pricePaid;
        PurchaseDate = DateTime.UtcNow;
    }
}
