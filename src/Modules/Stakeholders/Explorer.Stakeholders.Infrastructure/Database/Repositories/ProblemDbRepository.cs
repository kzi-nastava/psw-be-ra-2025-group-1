using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class ProblemDbRepository : IProblemRepository
{
    protected readonly StakeholdersContext DbContext;
    private readonly DbSet<Problem> _dbSet;

    public ProblemDbRepository(StakeholdersContext dbContext)
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

    public PagedResult<Problem> GetByCreatorId(long creatorId, int page, int pageSize)
    {
        var query = _dbSet.Where(p => p.CreatorId == creatorId);
        var task = query.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
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
            var existingProblem = Get(problem.Id);
            existingProblem.Update(problem.Priority, problem.Description, problem.Category);
            DbContext.SaveChanges();
            return existingProblem;
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
    }
}
