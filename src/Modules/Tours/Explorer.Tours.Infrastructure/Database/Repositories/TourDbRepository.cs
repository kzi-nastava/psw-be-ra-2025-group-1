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
            .Include(t => t.Equipment)
            .Include(t => t.TransportTimes)
            .Include(t => t.MapMarker)
            .FirstOrDefault(t => t.Id == id)
            ?? throw new NotFoundException($"Tour {id} not found");
    }

    public PagedResult<Tour> GetByCreatorId(long creatorId, int page, int pageSize)
    {
        var query = _dbSet
            .Include(t => t.Keypoints)
            .Include(t => t.Equipment)
            .Include(t => t.TransportTimes)
            .Include(t => t.MapMarker)
            .Where(t => t.CreatorId == creatorId);
        var task = query.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public List<Tour> GetAllByCreatorId(long creatorId)
    {
        return _dbSet
            .Include(t => t.Keypoints)
            .Include(t => t.Equipment)
            .Include(t => t.TransportTimes)
            .Include(t => t.MapMarker)
            .Where(t => t.CreatorId == creatorId)
            .ToList()
            ?? throw new NotFoundException($"Tours by author {creatorId} not found");
    }


    public PagedResult<Tour> GetPaged(int page, int pageSize)
    {
        var task = _dbSet
            .Include(t => t.Keypoints)
            .Include(t => t.Equipment)
            .Include(t => t.TransportTimes)
            .Include(t => t.MapMarker)
            .GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public Tour Update(Tour tour)
    {
        var existing = _dbSet
            .Include(t => t.MapMarker)
            .First(t => t.Id == tour.Id)
            ?? throw new NotFoundException($"Tour {tour.Id} not found");

        dbContext.Update(existing);
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

    public void DeleteMapMarker(MapMarker marker)
    {
        dbContext.MapMarkers.Remove(marker);
    }

}
