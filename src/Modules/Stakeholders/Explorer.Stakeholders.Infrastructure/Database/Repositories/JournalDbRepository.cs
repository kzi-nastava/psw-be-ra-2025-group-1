using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class JournalDbRepository : IJournalRepository
{
    private readonly StakeholdersContext _db;

    public JournalDbRepository(StakeholdersContext db)
    {
        _db = db;
    }

    public Journal Add(Journal journal)
    {
        _db.Journals.Add(journal);
        _db.SaveChanges();
        return journal;
    }

    public Journal? GetById(long id)
        => _db.Journals
            .Include(j => j.Collaborators)
                .ThenInclude(c => c.User)
            .FirstOrDefault(j => j.Id == id);

    public IEnumerable<Journal> GetByUserId(long userId)
        => _db.Journals.AsNoTracking()
            .Where(j => j.UserId == userId)
            .OrderByDescending(j => j.CreatedAt)
            .ToList();

    public void Update(Journal journal)
    {
        _db.Journals.Update(journal);
        _db.SaveChanges();
    }

    public void Delete(Journal journal)
    {
        _db.Journals.Remove(journal);
        _db.SaveChanges();
    }

    public (IReadOnlyList<Journal> Items, int TotalCount) GetPaged(int page, int size)
    {
        var query = _db.Journals.AsNoTracking().OrderByDescending(j => j.CreatedAt);

        var totalCount = query.Count();
        var items = query.Skip((page - 1) * size).Take(size).ToList();

        return (Items: items, TotalCount: totalCount);
    }

    public Journal? GetSingleByUserId(long userId)
        => _db.Journals.AsNoTracking()
            .FirstOrDefault(j => j.UserId == userId);


    public IEnumerable<Journal> GetAccessibleByUserId(long userId)
    {
        return _db.Journals
            .AsNoTracking()
            .Include(j => j.Collaborators)
                .ThenInclude(c => c.User)
            .Where(j => j.UserId == userId || j.Collaborators.Any(c => c.UserId == userId))
            .OrderByDescending(j => j.CreatedAt)
            .ToList();
    }

}