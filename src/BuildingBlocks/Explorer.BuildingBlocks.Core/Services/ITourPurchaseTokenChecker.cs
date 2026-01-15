namespace Explorer.BuildingBlocks.Core.Services;

public interface ITourPurchaseTokenChecker
{
    bool ExistsForUserAndTour(long userId, long tourId);
}
