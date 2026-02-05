using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourRatingReactionDbRepository : ITourRatingReactionRepository
    {
        protected readonly ToursContext DbContext;
        private readonly DbSet<TourRatingReaction> _dbSet;

        public TourRatingReactionDbRepository(ToursContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<TourRatingReaction>();
        }

        public PagedResult<TourRatingReaction> GetPaged(int page, int pageSize)
        {
            var task = _dbSet.GetPagedById(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public PagedResult<TourRatingReaction> GetPagedByTourRating(long tourRatingId, int page, int pageSize)
        {
            var task = _dbSet.GetPagedById(page, pageSize);
            task.Wait();
            List<TourRatingReaction> ratingReactions = task.Result.Results.Where(x => x.TourRatingId == tourRatingId).ToList();
            PagedResult<TourRatingReaction> result = new PagedResult<TourRatingReaction>(ratingReactions, ratingReactions.Count);
            return result;
        }

        public TourRatingReaction Get(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new NotFoundException("Not found: " + id);
            return entity;
        }

        public TourRatingReaction Create(TourRatingReaction entity)
        {
            _dbSet.Add(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public TourRatingReaction Update(TourRatingReaction entity)
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

        public bool Exists(long tourRatingId, long userId)
        {
            return _dbSet
                .Any(r => r.TourRatingId == tourRatingId && r.UserId == userId);
        }

    }
}