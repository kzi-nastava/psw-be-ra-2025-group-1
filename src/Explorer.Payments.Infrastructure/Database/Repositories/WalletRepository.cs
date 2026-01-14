
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        protected readonly PaymentsContext dbContext;
        private readonly DbSet<Wallet> _dbSet;

        public WalletRepository(PaymentsContext dbContext)
        {
            this.dbContext = dbContext;
            _dbSet = dbContext.Set<Wallet>();
        }

        public Wallet Create(Wallet wallet)
        {
            _dbSet.Add(wallet);
            dbContext.SaveChanges();
            return wallet;
        }

        public void Delete(long id)
        {
            var wallet = Get(id);

            if (wallet == null)
                return;

            _dbSet.Remove(wallet);
            dbContext.SaveChanges();
        }

        public bool ExistsByTouristId(long touristId)
        {
            Wallet? entity= _dbSet.FirstOrDefault(w => w.TouristId == touristId);
            return entity != null;
        }

        public Wallet? Get(long id)
        {
            return _dbSet
                .FirstOrDefault(w => w.Id == id)
                ?? throw new NotFoundException($"Wallet {id} not found");
        }

        public Wallet? GetByTouristId(long touristId)
        {
           return _dbSet.FirstOrDefault(w => w.TouristId == touristId)
                ?? throw new NotFoundException($"Wallet for Tourist {touristId} not found");
        }

        public Wallet Update(Wallet tour)
        {
            _dbSet.Update(tour);
            dbContext.SaveChanges();
            return tour;
        }
    }
}
