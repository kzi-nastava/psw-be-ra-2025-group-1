using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class ProblemMessageDbRepository : IProblemMessageRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<ProblemMessage> _dbSet;

    public ProblemMessageDbRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<ProblemMessage>();
    }

    public ProblemMessage Add(ProblemMessage message)
    {
        _dbSet.Add(message);
        DbContext.SaveChanges();
        return message;
    }

    public IEnumerable<ProblemMessage> GetByProblemId(long problemId)
    {
        return _dbSet
            .Where(pm => pm.ProblemId == problemId)
            .OrderBy(pm => pm.CreatedAt)
            .ToList();
    }

    public ProblemMessage? Get(long id)
    {
        return _dbSet.Find(id);
    }

    public ProblemMessage Update(ProblemMessage message)
    {
        _dbSet.Update(message);
        DbContext.SaveChanges();
        return message;
    }

    public void Delete(ProblemMessage message)
    {
        _dbSet.Remove(message);
        DbContext.SaveChanges();
    }
}
