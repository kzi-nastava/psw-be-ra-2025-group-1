using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;


namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class UserLocationDbRepository : IUserLocationRepository
    {

        protected readonly StakeholdersContext DbContext;
        private readonly DbSet<UserLocation> _dbSet;

        public UserLocationDbRepository(StakeholdersContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<UserLocation>();
        }

        public UserLocation Create(UserLocation entity)
        {
            _dbSet.Add(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public bool Delete(long id)
        {
            var tour = Get(id);

            if (tour == null)
                return false;

            _dbSet.Remove(tour);
            DbContext.SaveChanges();
            return true;
        }

        public UserLocation Get(long id)
        {
            var entity = _dbSet.Find(id) ?? throw new NotFoundException("Not found: " + id);
            return entity;
        }

        public List<UserLocation> GetByUserId(long userId)
        {
            var entity = _dbSet.Where(ul => ul.UserId == userId).ToList() ?? throw new NotFoundException("Not found: " + userId);
            return entity;
        }

        public UserLocation Update(UserLocation entity)
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
    }
}
