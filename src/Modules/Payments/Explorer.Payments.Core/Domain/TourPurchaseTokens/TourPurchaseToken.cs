using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain.TourPurchaseTokens;

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

    public bool IsValid => Status == TourPurchaseTokenStatus.Active;

    protected TourPurchaseToken() { }

    public TourPurchaseToken(long tourId, long userId, DateOnly purchaseDate)
    {
        if (tourId <= 0)
            throw new ArgumentException("TourId must be greater than zero.", nameof(tourId));

        if (userId <= 0)
            throw new ArgumentException("UserId must be greater than zero.", nameof(userId));

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
