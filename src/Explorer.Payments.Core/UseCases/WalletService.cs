
using Explorer.Payments.API.Dtos.ShoppingCart;
using Explorer.Payments.API.Public.Tourist;

namespace Explorer.Payments.Core.UseCases
{
    public class WalletService : IWalletService
    {
        public WalletDto Create(long touristId)
        {
            throw new NotImplementedException();
        }

        public WalletDto GetById(long walletId)
        {
            throw new NotImplementedException();
        }

        public WalletDto GetByTouristId(long touristId)
        {
            throw new NotImplementedException();
        }

        public WalletDto ResetBalance(long walletId)
        {
            throw new NotImplementedException();
        }

        public WalletDto UpdateBalance(long walletId, double newBalance)
        {
            throw new NotImplementedException();
        }
    }
}
