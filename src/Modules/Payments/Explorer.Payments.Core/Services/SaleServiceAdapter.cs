using Explorer.BuildingBlocks.Core.Services;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.Services;

public class SaleServiceAdapter : ISaleService
{
    private readonly ISaleRepository _saleRepository;

    public SaleServiceAdapter(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public List<ISaleInfo> GetActiveSalesForTour(long tourId)
    {
        var sales = _saleRepository.GetActiveSalesForTour(tourId);
        return sales.Select(s => new SaleInfo
        {
            Id = s.Id,
            Name = s.Name,
            DiscountPercentage = s.DiscountPercentage,
            TourIds = s.TourIds
        }).Cast<ISaleInfo>().ToList();
    }

    public List<ISaleInfo> GetActiveSales()
    {
        var sales = _saleRepository.GetActiveSales();
        return sales.Select(s => new SaleInfo
        {
            Id = s.Id,
            Name = s.Name,
            DiscountPercentage = s.DiscountPercentage,
            TourIds = s.TourIds
        }).Cast<ISaleInfo>().ToList();
    }
}
