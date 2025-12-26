using Explorer.Payments.Core.Domain;

public interface ISaleHistoryRepository
{
    SaleHistory Add(SaleHistory sale);
    SaleHistory? GetById(long id);
    List<SaleHistory> GetByTouristId(long touristId);
}
