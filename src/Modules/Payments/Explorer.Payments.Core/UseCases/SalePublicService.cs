using Explorer.Payments.API.Dtos.Sales;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases;

public class SalePublicService : ISalePublicService
{
    private readonly ISaleRepository _saleRepository;

    public SalePublicService(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public List<SaleDto> GetActiveSales()
    {
        var sales = _saleRepository.GetActiveSales();
        return sales.Select(MapToDto).ToList();
    }

    public SaleDto Get(long id)
    {
        var sale = _saleRepository.Get(id);
        if (sale == null)
            throw new ArgumentException("Sale not found.");

        return MapToDto(sale);
    }

    private SaleDto MapToDto(Core.Domain.Sales.Sale sale)
    {
        return new SaleDto
        {
            Id = sale.Id,
            Name = sale.Name,
            DiscountPercentage = sale.DiscountPercentage,
            StartDate = sale.StartDate,
            EndDate = sale.EndDate,
            AuthorId = sale.AuthorId,
            TourIds = sale.TourIds,
            IsActive = sale.IsActive()
        };
    }
}