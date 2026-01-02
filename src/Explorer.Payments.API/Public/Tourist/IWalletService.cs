

using Explorer.Payments.API.Dtos.ShoppingCart;

namespace Explorer.Payments.API.Public.Tourist
{
    public interface IWalletService
    {
        public WalletDto GetById(long walletId);
        public WalletDto GetByTouristId(long touristId);
        public WalletDto Create(long touristId);
        public WalletDto UpdateBalance(long walletId, double newBalance);
        public WalletDto ResetBalance(long walletId);
        public WalletDto Delete(long walletId);
    }
}
