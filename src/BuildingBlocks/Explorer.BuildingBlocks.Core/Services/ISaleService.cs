namespace Explorer.BuildingBlocks.Core.Services;

public interface ISaleService
{
    List<ISaleInfo> GetActiveSalesForTour(long tourId);
    List<ISaleInfo> GetActiveSales();
}
