using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourRatingDbRepository : IRatingRepository
    {
        protected readonly ToursContext DbContext;
        private readonly DbSet<TourRating> _dbSet;

        public TourRatingDbRepository(ToursContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<TourRating>();
        }

        public PagedResult<TourRating> GetPaged(int page, int pageSize)
        {
            var task = _dbSet.GetPagedById(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public PagedResult<TourRating> GetPagedByUser(int userId, int page, int pageSize)
        {
            var task = _dbSet.GetPagedById(page, pageSize);
            task.Wait();
            List<TourRating> userRatings = task.Result.Results.Where(x => x.UserId == userId).ToList();
            PagedResult<TourRating> result = new PagedResult<TourRating>(userRatings, userRatings.Count);
            return result;
        }

        public PagedResult<TourRating> GetPagedByTourExecution(int tourExecutionId, int page, int pageSize)
        {
            var task = _dbSet.GetPagedById(page, pageSize);
            task.Wait();
            List<TourRating> tourRatings = task.Result.Results.Where(x => x.TourExecutionId == tourExecutionId).ToList();
            PagedResult<TourRating> result = new PagedResult<TourRating>(tourRatings, tourRatings.Count);
            return result;
        }

        public TourRating Get(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new NotFoundException("Not found: " + id);
            return entity;
        }

        public TourRating Create(TourRating entity)
        {
            _dbSet.Add(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public TourRating Update(TourRating entity)
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

        public void Delete(long id)
        {
            var entity = Get(id);
            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }
    }
}