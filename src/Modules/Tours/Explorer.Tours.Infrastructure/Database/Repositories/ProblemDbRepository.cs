using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class ProblemDbRepository : IProblemRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<Problem> _dbSet;

    public ProblemDbRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Problem>();
    }
    public Problem Create(Problem problem)
    {
        _dbSet.Add(problem);
        DbContext.SaveChanges();
        return problem;
    }

    public void Delete(long id)
    {
        var problem = Get(id);
        _dbSet.Remove(problem);
        DbContext.SaveChanges();
    }

    public Problem Get(long id)
    {
        var problem = _dbSet.Find(id);
        if (problem == null) throw new NotFoundException("Not found: " + id);
        return problem;
    }

    public PagedResult<Problem> GetPaged(int page, int pageSize)
    {
        var task = _dbSet.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public Problem Update(Problem problem)
    {
        try
        {
            DbContext.Update(problem);
            DbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return problem;
    }
}
