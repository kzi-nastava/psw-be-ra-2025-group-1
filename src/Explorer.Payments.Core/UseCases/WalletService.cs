using AutoMapper;
using Explorer.Payments.API.Dtos.ShoppingCart;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Dtos;
namespace Explorer.Payments.Core.UseCases
{
    public class WalletService : IWalletService
    {
        protected readonly IWalletRepository walletRepository;
        protected readonly IMapper _mapper;

        public WalletService(IWalletRepository walletrepo, IMapper mapper)
        {
            walletRepository = walletrepo;
            this._mapper = mapper;
        }

        public WalletDto Create(long touristId)
        {
            var wallet = new Wallet(touristId, 0);
            var result = walletRepository.Create(wallet);
            return _mapper.Map<WalletDto>(wallet);
        }

        public WalletDto GetById(long walletId)
        {
            var wallet = walletRepository.Get(walletId);
            if (wallet == null) throw new KeyNotFoundException("Wallet not found.");
            return _mapper.Map<WalletDto>(wallet);
        }

        public WalletDto GetByTouristId(long touristId)
        {
            var wallet = walletRepository.GetByTouristId(touristId);
            if (wallet == null) throw new KeyNotFoundException("Wallet not found for this tourist.");
            return _mapper.Map<WalletDto>(wallet);
        }

        public WalletDto UpdateBalance(long walletId, double newBalance)
        {
            var wallet = walletRepository.Get(walletId);
            if (wallet == null) throw new KeyNotFoundException("Wallet not found.");

            wallet.Update(newBalance);

            var result = walletRepository.Update(wallet);
            return _mapper.Map<WalletDto>(wallet);
        }

        public WalletDto ResetBalance(long walletId)
        {
            return UpdateBalance(walletId, 0);
        }

        public WalletDto Delete(long walletId)
        {
            var wallet = walletRepository.Get(walletId);
            if (wallet == null) throw new KeyNotFoundException("Wallet not found.");

            walletRepository.Delete(walletId);
            return _mapper.Map<WalletDto>(wallet);
        }
    }
}