using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class RatingDbRepository : IRatingRepository
{
    private readonly StakeholdersContext _db;

    public RatingDbRepository(StakeholdersContext db)
    {
        _db = db;
    }

    public Rating? GetSingleByUserId(long userId)
        => _db.Ratings.AsNoTracking().FirstOrDefault(r => r.UserId == userId);

    public IEnumerable<Rating> GetByUserId(long userId)
        => _db.Ratings.AsNoTracking()
                      .Where(r => r.UserId == userId)
                      .OrderByDescending(r => r.CreatedAt)
                      .ToList();
    
    public (IReadOnlyList<Rating> Items, int TotalCount) GetPaged(int page, int size)
    {
        var query = _db.Ratings.AsNoTracking().OrderByDescending(r => r.CreatedAt);
        var totalCount = query.Count();
        var items = query.Skip((page - 1) * size).Take(size).ToList();
        return (Items: items, TotalCount: totalCount);
    }

    public Rating Add(Rating rating)
    {
        _db.Ratings.Add(rating);
        _db.SaveChanges();
        return rating;
    }

    public Rating? GetById(long id)
        => _db.Ratings.FirstOrDefault(r => r.Id == id);

    public void Update(Rating rating)
    {
        _db.Ratings.Update(rating);
        _db.SaveChanges();
    }

    public void Delete(Rating rating)
    {
        _db.Ratings.Remove(rating);
        _db.SaveChanges();
    }
}
