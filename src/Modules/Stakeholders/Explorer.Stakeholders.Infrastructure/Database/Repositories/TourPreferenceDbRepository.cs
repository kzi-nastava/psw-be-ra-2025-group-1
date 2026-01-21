using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;


namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class TourPreferenceDbRepository : ITourPreferenceRepository
    {
        protected readonly StakeholdersContext DbContext;
        private readonly DbSet<TourPreference> _dbSet;

        public TourPreferenceDbRepository(StakeholdersContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<TourPreference>();
        }

        public TourPreference Create(TourPreference entity)
        {
            _dbSet.Add(entity);
            DbContext.SaveChanges();
            return entity;
        }
        public TourPreference? Get(long id) => _dbSet.Find(id);

        public TourPreference Update(TourPreference entity)
        {
            try
            {
                DbContext.Update(entity);
                DbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }
            return entity;
        }
        public TourPreference? GetByUser(long userId) => _dbSet.FirstOrDefault(p => p.UserId == userId);
    }
}
