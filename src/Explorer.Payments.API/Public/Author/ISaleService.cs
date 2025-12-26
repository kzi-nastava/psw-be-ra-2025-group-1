using Explorer.Payments.API.Dtos.Sales;

namespace Explorer.Payments.API.Public.Author;

public interface ISaleService
{
    SaleDto Create(CreateSaleDto saleDto, long authorId);
    SaleDto Update(long id, UpdateSaleDto saleDto, long authorId);
    void Delete(long id, long authorId);
    SaleDto Get(long id);
    List<SaleDto> GetByAuthor(long authorId);
}