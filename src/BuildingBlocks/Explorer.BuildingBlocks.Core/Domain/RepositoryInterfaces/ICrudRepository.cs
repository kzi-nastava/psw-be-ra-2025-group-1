using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.BuildingBlocks.Core.Domain.RepositoryInterfaces;

public interface ICrudRepository<T> where T : Entity
{
    PagedResult<T> GetPaged(int page, int pageSize);
    T? Get(long id);
    T Create(T entity);
    T Update(T entity);
    void Delete(long id);
}
