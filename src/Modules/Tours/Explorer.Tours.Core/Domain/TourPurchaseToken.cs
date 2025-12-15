using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain.TourPurchaseTokens;

public enum TourPurchaseTokenStatus
{
    Active,
    Used,
    Expired
}

public class TourPurchaseToken : AggregateRoot
{
    public long TourId { get; private set; }
    public long UserId { get; private set; }
    public DateOnly PurchaseDate { get; private set; }
    public TourPurchaseTokenStatus Status { get; private set; }

    // Ovo AutoMapper može da mapira bez problema (bool ⇄ bool)
    public bool IsValid => Status == TourPurchaseTokenStatus.Active;

    // EF Core traži prazan konstruktor – neka bude protected
    protected TourPurchaseToken() { }

    public TourPurchaseToken(long tourId, long userId, DateOnly purchaseDate)
    {
        // NOVO - dozvoli negativne ID-jeve za testiranje:
        if (tourId == 0) throw new ArgumentException("TourId must not be zero.", nameof(tourId));
        if (userId == 0) throw new ArgumentException("UserId must not be zero.", nameof(userId));

        TourId = tourId;
        UserId = userId;
        PurchaseDate = purchaseDate;
        Status = TourPurchaseTokenStatus.Active;
    }

    public void MarkAsUsed()
    {
        if (Status != TourPurchaseTokenStatus.Active)
            throw new InvalidOperationException("Only active tokens can be marked as used.");

        Status = TourPurchaseTokenStatus.Used;
    }

    public void Expire()
    {
        if (Status == TourPurchaseTokenStatus.Expired) return;

        Status = TourPurchaseTokenStatus.Expired;
    }
}