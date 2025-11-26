namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IRatingRepository
{
    Rating Add(Rating rating);
    Rating? GetById(long id);
    IEnumerable<Rating> GetByUserId(long userId);

    void Update(Rating rating);
    void Delete(Rating rating);

    (IReadOnlyList<Rating> Items, int TotalCount) GetPaged(int page, int size);  // Admin list (paginacija)

    Rating? GetSingleByUserId(long userId);
}
