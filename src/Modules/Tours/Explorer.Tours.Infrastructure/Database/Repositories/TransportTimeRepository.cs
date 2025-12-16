using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TransportTimeRepository : ITransportTimeRepository
    {
        private readonly ToursContext DbContext;
        private readonly DbSet<TransportTime> _dbSet;

        public TransportTimeRepository(ToursContext toursContext)
        {
            DbContext = toursContext;
            _dbSet = DbContext.Set<TransportTime>();
        }
        public TransportTime Create(TransportTime entity)
        {
            _dbSet.Add(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public void Delete(long id)
        {
            var entity = Get(id);
            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }

        public TransportTime Get(long id)
        {
            var entity = _dbSet.Find(id);
            return entity ?? throw new NotFoundException("Not found: " + id);
        }

        public IEnumerable<TransportTime> GetByTourId(long tourId)
        {
            return _dbSet.AsNoTracking()
                        .Where(t => t.TourId == tourId)
                        .ToList();
        }

        public IEnumerable<TransportTime> GetByTransportType(TransportType transport)
        {
           return _dbSet.AsNoTracking()
                      .Where(t => t.Type==transport)
                      .ToList();
        }

        public PagedResult<TransportTime> GetPaged(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var task = _dbSet.GetPagedById(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public TransportTime Update(TransportTime entity)
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
