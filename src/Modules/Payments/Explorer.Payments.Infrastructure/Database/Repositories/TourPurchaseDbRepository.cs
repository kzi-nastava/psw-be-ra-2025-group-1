using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories;

public class TourPurchaseDbRepository : ITourPurchaseRepository
{
    private readonly PaymentsContext _dbContext;
    private readonly DbSet<TourPurchase> _dbSet;

    public TourPurchaseDbRepository(PaymentsContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<TourPurchase>();
    }

    public TourPurchase Create(TourPurchase tourPurchase)
    {
        _dbSet.Add(tourPurchase);
        _dbContext.SaveChanges();
        return tourPurchase;
    }

    public bool HasPurchased(long touristId, long tourId)
    {
        return _dbSet.Any(tp => tp.TouristId == touristId && tp.TourId == tourId);
    }

    public List<TourPurchase> GetByTourist(long touristId)
    {
        return _dbSet.Where(tp => tp.TouristId == touristId).ToList();
    }
}
