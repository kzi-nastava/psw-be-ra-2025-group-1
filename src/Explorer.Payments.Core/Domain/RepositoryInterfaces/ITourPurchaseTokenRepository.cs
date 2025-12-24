using Explorer.Payments.Core.Domain.TourPurchaseTokens;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface ITourPurchaseTokenRepository
    {
        TourPurchaseToken Create(TourPurchaseToken token);

        TourPurchaseToken Update(TourPurchaseToken token);

        TourPurchaseToken? Get(long id);

        List<TourPurchaseToken> GetByUserId(long userId);
        
        bool ExistsForUserAndTour(long userId, long tourId);
    }
}