using Explorer.Payments.API.Dtos.Sales;

namespace Explorer.Payments.API.Public.Tourist;

public interface ISalePublicService
{
    List<SaleDto> GetActiveSales();
    SaleDto Get(long id);
}