using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class ProblemDeadlineDbRepository : IProblemDeadlineRepository
{
    private readonly StakeholdersContext _dbContext;
    private readonly DbSet<ProblemDeadline> _dbSet;

    public ProblemDeadlineDbRepository(StakeholdersContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<ProblemDeadline>();
    }

    public ProblemDeadline Create(ProblemDeadline deadline)
    {
        _dbSet.Add(deadline);
        _dbContext.SaveChanges();
        return deadline;
    }

    public ProblemDeadline? GetByProblemId(long problemId)
    {
        return _dbSet.FirstOrDefault(d => d.ProblemId == problemId);
    }

    public ProblemDeadline? GetLatestByProblemId(long problemId)
    {
        return _dbSet
            .Where(d => d.ProblemId == problemId)
            .OrderByDescending(d => d.SetAt)
            .FirstOrDefault();
    }

    public List<ProblemDeadline> GetExpiredDeadlines()
    {
        var now = DateTime.UtcNow;
        return _dbSet
            .Where(d => d.DeadlineDate < now)
            .OrderBy(d => d.DeadlineDate)
            .ToList();
    }
}
