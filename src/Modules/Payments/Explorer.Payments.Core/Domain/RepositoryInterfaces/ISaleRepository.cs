using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.Core.Domain.Sales;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces;

public interface ISaleRepository
{
    Sale Create(Sale sale);
    Sale Update(Sale sale);
    void Delete(Sale sale);
    Sale? Get(long id);
    PagedResult<Sale> GetByAuthor(long authorId, int page, int pageSize);
    List<Sale> GetActiveSales();
    List<Sale> GetActiveSalesForTour(long tourId);
}