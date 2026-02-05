using Explorer.Payments.Core.Domain.Bundles;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Infrastructure.Database;

namespace Explorer.Payments.Infrastructure.Database.Repositories;

public class BundlePurchaseRepository : IBundlePurchaseRepository
{
    private readonly PaymentsContext _dbContext;

    public BundlePurchaseRepository(PaymentsContext dbContext)
    {
        _dbContext = dbContext;
    }

    public BundlePurchase Create(BundlePurchase purchase)
    {
        _dbContext.Set<BundlePurchase>().Add(purchase);
        _dbContext.SaveChanges();
        return purchase;
    }

    public List<BundlePurchase> GetByTouristId(long touristId)
    {
        return _dbContext.Set<BundlePurchase>().Where(p => p.TouristId == touristId).ToList();
    }
}
