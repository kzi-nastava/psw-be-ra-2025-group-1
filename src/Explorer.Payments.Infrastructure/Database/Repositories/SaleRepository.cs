using Explorer.Payments.Core.Domain.Sales;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Infrastructure.Database.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly PaymentsContext _context;

    public SaleRepository(PaymentsContext context)
    {
        _context = context;
    }

    public Sale Create(Sale sale)
    {
        _context.Sales.Add(sale);
        _context.SaveChanges();
        return sale;
    }

    public Sale Update(Sale sale)
    {
        _context.Sales.Update(sale);
        _context.SaveChanges();
        return sale;
    }

    public void Delete(Sale sale)
    {
        _context.Sales.Remove(sale);
        _context.SaveChanges();
    }

    public Sale? Get(long id)
    {
        return _context.Sales.FirstOrDefault(s => s.Id == id);
    }

    public List<Sale> GetByAuthor(long authorId)
    {
        return _context.Sales
            .Where(s => s.AuthorId == authorId)
            .OrderByDescending(s => s.StartDate)
            .ToList();
    }

    public List<Sale> GetActiveSales()
    {
        var now = DateTime.UtcNow;
        return _context.Sales
            .Where(s => s.StartDate <= now && s.EndDate >= now)
            .ToList();
    }

    public List<Sale> GetActiveSalesForTour(long tourId)
    {
        var now = DateTime.UtcNow;
        return _context.Sales
            .Where(s => s.StartDate <= now
                     && s.EndDate >= now
                     && s.TourIds.Contains(tourId))
            .ToList();
    }
}