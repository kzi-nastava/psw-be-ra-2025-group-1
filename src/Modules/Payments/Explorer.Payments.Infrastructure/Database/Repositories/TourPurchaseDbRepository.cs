using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourPurchaseDbRepository : ITourPurchaseRepository
{
    private readonly ToursContext _dbContext;
    private readonly DbSet<TourPurchase> _dbSet;

    public TourPurchaseDbRepository(ToursContext dbContext)
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
        return _dbSet.Any(p => p.TouristId == touristId && p.TourId == tourId);
    }

    public List<TourPurchase> GetByTourist(long touristId)
    {
        return _dbSet.Where(p => p.TouristId == touristId).ToList();
    }
}
