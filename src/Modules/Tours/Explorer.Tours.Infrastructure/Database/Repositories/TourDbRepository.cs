using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourDbRepository : ITourRepository
{
    protected readonly ToursContext dbContext;
    private readonly DbSet<Tour> _dbSet;

    public TourDbRepository(ToursContext dbContext)
    {
        this.dbContext = dbContext;
        _dbSet = dbContext.Set<Tour>();
    }

    public Tour Create(Tour tour)
    {
        _dbSet.Add(tour);
        dbContext.SaveChanges();
        return tour;
    }

    public void Delete(long id)
    {
        var tour = Get(id);

        if (tour == null)
            return;

        _dbSet.Remove(tour);
        dbContext.SaveChanges();
    }

    public Tour? Get(long id)
    {
        return _dbSet
            .Include(t => t.Keypoints)
            .FirstOrDefault(t => t.Id == id)
            ?? throw new NotFoundException($"Tour {id} not found");
    }

    public PagedResult<Tour> GetByCreatorId(long creatorId, int page, int pageSize)
    {
        var query = _dbSet
            .Include(t => t.Keypoints)
            .Where(t => t.CreatorId == creatorId);
        var task = query.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public PagedResult<Tour> GetPaged(int page, int pageSize)
    {
        var task = _dbSet
            .Include(t => t.Keypoints)
            .GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public Tour Update(Tour tour)
    {
        _dbSet.Update(tour);
        dbContext.SaveChanges();
        return tour;
    }

    public PagedResult<Tour> GetPublished(int page, int pageSize)
    {
        var query = _dbSet.Where(t => t.Status == TourStatus.Published);

        var task = query.GetPaged(page, pageSize);
        task.Wait();

        return task.Result;
    }


    public Tour? GetPublishedById(long id)
    {
        return _dbSet
            .FirstOrDefault(t => t.Id == id && t.Status == TourStatus.Published);
    }

}
