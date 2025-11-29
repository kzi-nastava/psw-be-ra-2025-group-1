using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class EquipmentDbRepository : IEquipmentRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<Equipment> _dbSet;

    public EquipmentDbRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Equipment>();
    }

    public PagedResult<Equipment> GetPaged(int page, int pageSize)
    {
        // Ensure valid pagination parameters
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;
        
        // Log the query being executed
        Console.WriteLine($"[EquipmentDbRepository] GetPaged called with page={page}, pageSize={pageSize}");
        
        // Get total count first
        var totalCount = _dbSet.Count();
        Console.WriteLine($"[EquipmentDbRepository] Total count in database: {totalCount}");
        
        var task = _dbSet.GetPagedById(page, pageSize);
        task.Wait();
        
        Console.WriteLine($"[EquipmentDbRepository] Retrieved {task.Result.Results.Count} items");
        
        return task.Result;
    }

    public Equipment Get(long id)
    {
        var entity = _dbSet.Find(id);
        if (entity == null) throw new NotFoundException("Not found: " + id);
        return entity;
    }

    public Equipment Create(Equipment entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public Equipment Update(Equipment entity)
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
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }
}