namespace Explorer.BuildingBlocks.Core.Services;

public interface ISaleInfo
{
    long Id { get; }
    string Name { get; }
    int DiscountPercentage { get; }
    List<long> TourIds { get; }
}
