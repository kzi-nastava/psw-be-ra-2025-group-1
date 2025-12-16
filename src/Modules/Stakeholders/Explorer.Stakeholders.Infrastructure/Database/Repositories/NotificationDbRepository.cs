using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class NotificationDbRepository : INotificationRepository
{
    protected readonly StakeholdersContext DbContext;
    private readonly DbSet<Notification> _dbSet;

    public NotificationDbRepository(StakeholdersContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Notification>();
    }

    public Notification Create(Notification notification)
    {
        _dbSet.Add(notification);
        DbContext.SaveChanges();
        return notification;
    }

    public Notification? Get(long id)
    {
        return _dbSet.Find(id);
    }

    public PagedResult<Notification> GetByUserId(long userId, int page, int pageSize)
    {
        var query = _dbSet
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.Timestamp);
        
        var task = query.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public List<Notification> GetUnreadByUserId(long userId)
    {
        return _dbSet
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.Timestamp)
            .ToList();
    }

    public Notification Update(Notification notification)
    {
        try
        {
            DbContext.Update(notification);
            DbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return notification;
    }

    public void Delete(long id)
    {
        var entity = Get(id);
        if (entity == null)
            throw new NotFoundException($"Notification with id {id} not found.");
        
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }
}
