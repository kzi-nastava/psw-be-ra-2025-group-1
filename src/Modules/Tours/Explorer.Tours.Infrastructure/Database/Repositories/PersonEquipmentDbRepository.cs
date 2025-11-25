using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class PersonEquipmentDbRepository : IPersonEquipmentRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<PersonEquipment> _dbSet;

    public PersonEquipmentDbRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<PersonEquipment>();
    }

    public PagedResult<PersonEquipment> GetPaged(int page, int pageSize)
    {
        var task = _dbSet.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public PagedResult<PersonEquipment> GetByPersonId(long personId, int page, int pageSize)
    {
        var query = _dbSet.Where(pe => pe.PersonId == personId);
        var task = query.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public PersonEquipment? GetByPersonAndEquipment(long personId, long equipmentId)
    {
        return _dbSet.FirstOrDefault(pe => pe.PersonId == personId && pe.EquipmentId == equipmentId);
    }

    public PersonEquipment Add(PersonEquipment personEquipment)
    {
        _dbSet.Add(personEquipment);
        DbContext.SaveChanges();
        return personEquipment;
    }

    public void Remove(long personId, long equipmentId)
    {
        var entity = _dbSet.FirstOrDefault(pe => pe.PersonId == personId && pe.EquipmentId == equipmentId);
        if (entity == null) throw new NotFoundException($"PersonEquipment not found");

        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }
}