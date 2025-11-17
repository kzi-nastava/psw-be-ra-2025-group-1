using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        throw new NotImplementedException();
    }

    public void Delete(long id)
    {
        throw new NotImplementedException();
    }

    public Tour Get(long id)
    {
        throw new NotImplementedException();
    }

    public PagedResult<Tour> GetByCreatorId(long creatorId, int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public PagedResult<Tour> GetPaged(int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public Tour Update(Tour tour)
    {
        throw new NotImplementedException();
    }
}
