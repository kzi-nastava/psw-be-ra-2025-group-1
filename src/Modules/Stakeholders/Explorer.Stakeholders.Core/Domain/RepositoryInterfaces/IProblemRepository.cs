using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IProblemRepository
{
    Problem Create(Problem problem);
    Problem Update(Problem problem);
    Problem Get(long id);
    void Delete(long id);
    PagedResult<Problem> GetPaged(int page, int pageSize);
    PagedResult<Problem> GetByCreatorId(long creatorId, int page, int pageSize);
    PagedResult<Problem> GetByAuthorId(long authorId, int page, int pageSize);
    List<Problem> GetUnresolvedOlderThan(int days);
}
