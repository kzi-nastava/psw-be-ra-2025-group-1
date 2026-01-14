using Explorer.Payments.Core.Domain.Bundles;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces;

public interface IBundleRepository
{
    Bundle Create(Bundle bundle);
    Bundle Update(Bundle bundle);
    void Delete(long id);
    Bundle? Get(long id);
    List<Bundle> GetByAuthorId(long authorId);
    List<Bundle> GetAllPublished();
}
