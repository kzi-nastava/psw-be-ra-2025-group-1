using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Dtos.ShoppingCart;

namespace Explorer.Payments.API.Public.Tourist;

public interface ITouristSaleService
{
    List<SaleDto> GetByTouristId(long touristId);
}
