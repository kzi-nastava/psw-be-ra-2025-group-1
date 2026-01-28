using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.ProjectAutopsy.Core.Domain;
using Explorer.ProjectAutopsy.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.ProjectAutopsy.Infrastructure.Database.Repositories;

public class AutopsyProjectRepository : IAutopsyProjectRepository
{
    protected readonly ProjectAutopsyContext DbContext;
    private readonly DbSet<AutopsyProject> _dbSet;

    public AutopsyProjectRepository(ProjectAutopsyContext context)
    {
        DbContext = context;
        _dbSet = DbContext.Set<AutopsyProject>();
    }

    public PagedResult<AutopsyProject> GetPaged(int page, int pageSize)
    {
        var task = _dbSet.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public AutopsyProject? Get(long id)
    {
        return _dbSet.Find(id);
    }

    public AutopsyProject Create(AutopsyProject entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public AutopsyProject Update(AutopsyProject entity)
    {
        try
        {
            DbContext.Update(entity);
            DbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return entity;
    }

    public void Delete(long id)
    {
        var entity = Get(id);
        if (entity == null) throw new NotFoundException("Not found: " + id);
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }

    public AutopsyProject? GetByName(string name)
    {
        return DbContext.AutopsyProjects
            .FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
    }

    public IEnumerable<AutopsyProject> GetActive()
    {
        return DbContext.AutopsyProjects
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToList();
    }
}

public class RiskSnapshotRepository : IRiskSnapshotRepository
{
    protected readonly ProjectAutopsyContext DbContext;
    private readonly DbSet<RiskSnapshot> _dbSet;

    public RiskSnapshotRepository(ProjectAutopsyContext context)
    {
        DbContext = context;
        _dbSet = DbContext.Set<RiskSnapshot>();
    }

    public PagedResult<RiskSnapshot> GetPaged(int page, int pageSize)
    {
        var task = _dbSet.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public RiskSnapshot? Get(long id)
    {
        return _dbSet.Find(id);
    }

    public RiskSnapshot Create(RiskSnapshot entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public RiskSnapshot Update(RiskSnapshot entity)
    {
        try
        {
            DbContext.Update(entity);
            DbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return entity;
    }

    public void Delete(long id)
    {
        var entity = Get(id);
        if (entity == null) throw new NotFoundException("Not found: " + id);
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }

    public RiskSnapshot? GetLatestByProject(long projectId)
    {
        return DbContext.RiskSnapshots
            .Where(s => s.ProjectId == projectId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefault();
    }

    public IEnumerable<RiskSnapshot> GetByProject(long projectId)
    {
        return DbContext.RiskSnapshots
            .Where(s => s.ProjectId == projectId)
            .OrderByDescending(s => s.CreatedAt)
            .ToList();
    }

    public IEnumerable<RiskSnapshot> GetByProjectInRange(long projectId, DateTime start, DateTime end)
    {
        return DbContext.RiskSnapshots
            .Where(s => s.ProjectId == projectId && 
                        s.CreatedAt >= start && 
                        s.CreatedAt <= end)
            .OrderByDescending(s => s.CreatedAt)
            .ToList();
    }
}

public class AIReportRepository : IAIReportRepository
{
    protected readonly ProjectAutopsyContext DbContext;
    private readonly DbSet<AIReport> _dbSet;

    public AIReportRepository(ProjectAutopsyContext context)
    {
        DbContext = context;
        _dbSet = DbContext.Set<AIReport>();
    }

    public PagedResult<AIReport> GetPaged(int page, int pageSize)
    {
        var task = _dbSet.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public AIReport? Get(long id)
    {
        return _dbSet.Find(id);
    }

    public AIReport Create(AIReport entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public AIReport Update(AIReport entity)
    {
        try
        {
            DbContext.Update(entity);
            DbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return entity;
    }

    public void Delete(long id)
    {
        var entity = Get(id);
        if (entity == null) throw new NotFoundException("Not found: " + id);
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }

    public IEnumerable<AIReport> GetByProject(long projectId)
    {
        return DbContext.AIReports
            .Where(r => r.ProjectId == projectId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
    }

    public AIReport? GetLatestByProject(long projectId)
    {
        return DbContext.AIReports
            .Where(r => r.ProjectId == projectId)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefault();
    }

    public IEnumerable<AIReport> GetBySnapshot(long snapshotId)
    {
        return DbContext.AIReports
            .Where(r => r.RiskSnapshotId == snapshotId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
    }
}
