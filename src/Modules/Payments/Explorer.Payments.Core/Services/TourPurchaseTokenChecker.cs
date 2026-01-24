using Explorer.BuildingBlocks.Core.Services;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.Services;

public class TourPurchaseTokenChecker : ITourPurchaseTokenChecker
{
    private readonly ITourPurchaseTokenRepository _repo;

    public TourPurchaseTokenChecker(ITourPurchaseTokenRepository repo)
    {
        _repo = repo;
    }

    public bool ExistsForUserAndTour(long userId, long tourId)
        => _repo.ExistsForUserAndTour(userId, tourId);
}