using Explorer.Payments.Core.Domain.Bundles;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories;

public class BundleRepository : IBundleRepository
{
    private readonly PaymentsContext _dbContext;

    public BundleRepository(PaymentsContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Bundle Create(Bundle bundle)
    {
        _dbContext.Set<Bundle>().Add(bundle);
        _dbContext.SaveChanges();
        return bundle;
    }

    public Bundle Update(Bundle bundle)
    {
        _dbContext.Set<Bundle>().Update(bundle);
        _dbContext.SaveChanges();
        return bundle;
    }

    public void Delete(long id)
    {
        var bundle = Get(id);
        if (bundle != null)
        {
            _dbContext.Set<Bundle>().Remove(bundle);
            _dbContext.SaveChanges();
        }
    }

    public Bundle? Get(long id)
    {
        return _dbContext.Set<Bundle>().FirstOrDefault(b => b.Id == id);
    }

    public List<Bundle> GetByAuthorId(long authorId)
    {
        return _dbContext.Set<Bundle>().Where(b => b.AuthorId == authorId).ToList();
    }

    public List<Bundle> GetAllPublished()
    {
        return _dbContext.Set<Bundle>().Where(b => b.Status == BundleStatus.Published).ToList();
    }

    public List<Bundle> GetAll()
    {
        return _dbContext.Set<Bundle>().ToList();
    }
}
