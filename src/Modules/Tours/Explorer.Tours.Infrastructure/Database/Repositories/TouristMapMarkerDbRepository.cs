using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Explorer.BuildingBlocks.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TouristMapMarkerDbRepository : ITouristMapMarkerRepository
    {
        protected readonly ToursContext dbContext;
        private readonly DbSet<TouristMapMarker> _dbSet;

        public TouristMapMarkerDbRepository(ToursContext dbContext)
        {
            this.dbContext = dbContext;
            _dbSet = dbContext.Set<TouristMapMarker>();
        }

        public PagedResult<TouristMapMarker> GetPagedByTourist(int page, int pageSize, long touristId)
        {
            var query = _dbSet.Where(tm => tm.TouristId == touristId);

            var task = query.GetPagedById(page, pageSize);
            task.Wait();

            return task.Result;
        }

        public List<TouristMapMarker> GetAllByTourist(long touristId)
        {
            return _dbSet
                .Where(tm => tm.TouristId == touristId)
                .ToList();
        }

        public TouristMapMarker GetActiveByTourist(long touristId)
        {
            return _dbSet
                .FirstOrDefault(tm => tm.TouristId == touristId && tm.IsActive)
                ?? throw new NotFoundException($"Active map marker not found for tourist {touristId}");
        }

        public TouristMapMarker Create(TouristMapMarker touristMapMarker)
        {
            _dbSet.Add(touristMapMarker);
            dbContext.SaveChanges();
            return touristMapMarker;
        }

        public TouristMapMarker Update(TouristMapMarker updatedTouristMapMarker)
        {
            var existing = _dbSet.First(m => m.Id == updatedTouristMapMarker.Id) ?? throw new NotFoundException($"Tourist map marker {updatedTouristMapMarker.Id} not found");

            dbContext.Update(existing);
            dbContext.SaveChanges();
            return updatedTouristMapMarker;
        }

        public void Delete(long id)
        {
            var entity = _dbSet.FirstOrDefault(tm => tm.Id == id);

            if (entity == null)
                throw new NotFoundException($"TouristMapMarker {id} not found");

            _dbSet.Remove(entity);
            dbContext.SaveChanges();
        }
    }
}
