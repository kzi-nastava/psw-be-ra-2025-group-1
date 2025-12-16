namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourPurchaseRepository
{
    TourPurchase Create(TourPurchase tourPurchase);
    bool HasPurchased(long touristId, long tourId);
    List<TourPurchase> GetByTourist(long touristId);
}
