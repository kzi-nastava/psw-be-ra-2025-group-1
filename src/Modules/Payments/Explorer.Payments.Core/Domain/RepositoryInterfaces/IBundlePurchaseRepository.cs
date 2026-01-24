using Explorer.Payments.Core.Domain.Bundles;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces;

public interface IBundlePurchaseRepository
{
    BundlePurchase Create(BundlePurchase purchase);
    List<BundlePurchase> GetByTouristId(long touristId);
}
