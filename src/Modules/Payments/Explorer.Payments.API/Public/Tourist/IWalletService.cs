

using Explorer.Payments.API.Dtos.ShoppingCart;

namespace Explorer.Payments.API.Public.Tourist
{
    public interface IWalletService
    {
        public WalletDto GetById(long walletId);
        public WalletDto GetByTouristId(long touristId);
        public WalletDto Create(long touristId);
        public WalletDto UpdateBalance(long walletId, WalletDto request);
        public WalletDto Delete(long walletId);
    }
}
