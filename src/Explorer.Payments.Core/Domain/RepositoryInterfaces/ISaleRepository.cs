using Explorer.Payments.Core.Domain;

public interface ISaleRepository
{
    Sale Add(Sale sale);
    Sale? GetById(long id);
    List<Sale> GetByTouristId(long touristId);
}
