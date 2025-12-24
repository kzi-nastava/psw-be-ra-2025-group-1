using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Dtos.ShoppingCart;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;

    public SaleService(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public List<SaleDto> GetByTouristId(long touristId)
    {
        var sales = _saleRepository.GetByTouristId(touristId);

        return sales.Select(sale => new SaleDto
        {
            Id = sale.Id,
            PurchasedAt = sale.PurchasedAt,
            TotalPrice = sale.TotalPrice,
            Items = sale.Items.Select(item => new SaleItemDto
            {
                TourId = item.TourId,
                TourName = item.TourName,
                Price = item.Price,
                Quantity = item.Quantity
            }).ToList()
        }).ToList();
    }
}
