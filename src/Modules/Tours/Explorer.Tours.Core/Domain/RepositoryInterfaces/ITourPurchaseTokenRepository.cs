using Explorer.Tours.Core.Domain.TourPurchaseTokens;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourPurchaseTokenRepository
{
    TourPurchaseToken Create(TourPurchaseToken token);
    TourPurchaseToken Update(TourPurchaseToken token);
    TourPurchaseToken? Get(long id);
    List<TourPurchaseToken> GetByUserId(long userId);
    bool ExistsForUserAndTour(long userId, long tourId);
}
