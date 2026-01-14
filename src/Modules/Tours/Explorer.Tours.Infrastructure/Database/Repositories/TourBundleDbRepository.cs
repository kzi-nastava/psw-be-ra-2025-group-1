using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourBundleDbRepository : ITourBundleRepository
    {
        private readonly ToursContext _dbContext;
        private readonly DbSet<TourBundle> _dbSet;

        public TourBundleDbRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TourBundle>();
        }

        public TourBundle Create(TourBundle bundle)
        {
            _dbSet.Add(bundle);
            _dbContext.SaveChanges();
            return bundle;
        }

        public TourBundle Update(TourBundle bundle)
        {
            _dbSet.Update(bundle);
            _dbContext.SaveChanges();
            return bundle;
        }

        public void Delete(long id)
        {
            var bundle = Get(id);
            _dbSet.Remove(bundle);
            _dbContext.SaveChanges();
        }

        public TourBundle Get(long id)
        {
            return _dbSet.Include(b => b.Tours)
                         .FirstOrDefault(b => b.Id == id)
                         ?? throw new NotFoundException($"TourBundle {id} not found");
        }

        public List<TourBundle> GetByCreator(long creatorId)
        {
            return _dbSet.Include(b => b.Tours)
                         .Where(b => b.CreatorId == creatorId)
                         .ToList();
        }

        public List<TourBundle> GetAll()
        {
            return _dbSet.Include(b => b.Tours).ToList();
        }
    }
}
