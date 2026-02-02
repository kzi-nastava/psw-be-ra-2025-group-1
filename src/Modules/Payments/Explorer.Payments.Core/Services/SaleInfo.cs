using Explorer.BuildingBlocks.Core.Services;

namespace Explorer.Payments.Core.Services;

public class SaleInfo : ISaleInfo
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DiscountPercentage { get; set; }
    public List<long> TourIds { get; set; } = new();
}
