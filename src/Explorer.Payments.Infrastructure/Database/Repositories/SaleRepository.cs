using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
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

    public PagedResult<Sale> GetByAuthor(long authorId, int page, int pageSize)
    {
        var query = _context.Sales
            .Where(s => s.AuthorId == authorId)
            .OrderByDescending(s => s.StartDate);

        var task = query.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
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
        // Load all active sales and filter in memory since EF Core can't translate Contains on array
        return _context.Sales
            .Where(s => s.StartDate <= now && s.EndDate >= now)
            .AsEnumerable()  // Switch to client evaluation
            .Where(s => s.TourIds.Contains(tourId))
            .ToList();
    }
}