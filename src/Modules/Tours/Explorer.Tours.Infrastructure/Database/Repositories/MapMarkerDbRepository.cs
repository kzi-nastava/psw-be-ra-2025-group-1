using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class MapMarkerDbRepository : IMapMarkerRepository
    {
        protected readonly ToursContext DbContext;
        private readonly DbSet<MapMarker> _dbSet;

        public MapMarkerDbRepository(ToursContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<MapMarker>();
        }

        public PagedResult<MapMarker> GetPaged(int page, int pageSize)
        {
            var task = _dbSet
            .GetPagedById(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public List<MapMarker> GetAll()
        {
            return _dbSet.ToList();
        }

        public MapMarker Get(long mapMarkerId)
        {
            var marker = _dbSet.Find(mapMarkerId);
            if(marker == null)
            {
                throw new NotFoundException("Map marker not found: " + mapMarkerId);
            }

            return marker;
        }

        public MapMarker Create(MapMarker mapMarker)
        {
            _dbSet.Add(mapMarker);
            DbContext.SaveChanges();
            return mapMarker;
        }

        public MapMarker Update(MapMarker mapMarker)
        {
            try
            {
                DbContext.Update(mapMarker);
                DbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }
            return mapMarker;
        }

        public void Delete(long mapMarkerId)
        {
            var mapMarker = Get(mapMarkerId);
            _dbSet.Remove(mapMarker);
            DbContext.SaveChanges();
        }
    }
}
