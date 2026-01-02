using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IWalletRepository
    {
        Wallet Create(Wallet wallet);
        Wallet Update(Wallet tour);
        Wallet? Get(long id);
        Wallet? GetByTouristId(long touristId);
        void Delete(long id);
    }
}
