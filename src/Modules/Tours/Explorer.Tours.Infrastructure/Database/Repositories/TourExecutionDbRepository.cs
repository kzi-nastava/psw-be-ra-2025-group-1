using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourExecutionDbRepository : ITourExecutionRepository
{
    private readonly ToursContext _dbContext;
    private readonly DbSet<TourExecution> _dbSet;

    public TourExecutionDbRepository(ToursContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<TourExecution>();
    }

    public TourExecution Create(TourExecution tourExecution)
    {
        _dbSet.Add(tourExecution);
        _dbContext.SaveChanges();
        return tourExecution;
    }

    public TourExecution Update(TourExecution tourExecution)
    {
        try
        {
            _dbContext.Update(tourExecution);
            _dbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return tourExecution;
    }

    public TourExecution? Get(long id)
    {
        return _dbSet
            .Include(te => te.KeypointProgresses)
            .FirstOrDefault(te => te.Id == id);
    }

    public TourExecution? GetActiveTourByTourist(long touristId)
    {
        return _dbSet
            .Include(te => te.KeypointProgresses)
            .Where(te => te.TouristId == touristId && te.Status == TourExecutionStatus.InProgress)
            .OrderByDescending(te => te.StartTime)
            .FirstOrDefault();
    }

    public List<TourExecution> GetByTourist(long touristId)
    {
        return _dbSet
            .Include(te => te.KeypointProgresses)
            .Where(te => te.TouristId == touristId)
            .OrderByDescending(te => te.StartTime)
            .ToList();
    }

    public List<TourExecution> GetByTourId(long tourId)
    {
        return _dbSet
            .Include(te => te.KeypointProgresses)
            .Where(te => te.TourId == tourId)
            .OrderByDescending(te => te.StartTime)
            .ToList();
    }
}
