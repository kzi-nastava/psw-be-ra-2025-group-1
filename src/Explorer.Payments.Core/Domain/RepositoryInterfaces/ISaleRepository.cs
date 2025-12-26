using Explorer.Payments.Core.Domain.Sales;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces;

public interface ISaleRepository
{
    Sale Create(Sale sale);
    Sale Update(Sale sale);
    void Delete(Sale sale);
    Sale? Get(long id);
    List<Sale> GetByAuthor(long authorId);
    List<Sale> GetActiveSales();
    List<Sale> GetActiveSalesForTour(long tourId);
}